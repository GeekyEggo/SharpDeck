/**
 * Copied and simplified from PI Sample
 * https://github.com/elgatosf/streamdeck-pisamples/blob/master/Sources/com.elgato.pisamples.sdPlugin/js/index_pi.js
 **/

/// <reference path="./libs/js/property-inspector.js" />
/// <reference path="./libs/js/action.js" />
/// <reference path="./libs/js/utils.js" />

console.log('Property Inspector loaded', $PI);

// register a callback for the 'connected' event
// this is all you need to communicate with the plugin and the StreamDeck software
$PI.onConnected(jsn => {
    console.log('Property Inspector connected', jsn);
    initPropertyInspector();
    console.log(jsn.actionInfo.payload.settings);

    // Initialize the slider with current value in settings
    document.getElementById('tickmultiplier').getElementsByTagName("input")[0].value = jsn.actionInfo.payload.settings['tickMultiplier'];
});

/**
 * DEMO
 * ----
 * 
 * This initializes all elements found in the PI and makes them
 * interactive. It will also send the values to the plugin.
 * 
 * */

function initPropertyInspector(initDelay) {
    prepareDOMElements(document);
    /** expermimental carousel is not part of the DOM
     * so let the DOM get constructed first and then
     * inject the carousel */
    setTimeout(function () {
        initToolTips();
    }, initDelay || 100);
}

// our method to pass values to the plugin
function sendValueToPlugin(value, param) {
    $PI.sendToPlugin({ [param]: value });
}

/** CREATE INTERACTIVE HTML-DOM
 * where elements can be clicked or act on their 'change' event.
 * Messages are then processed using the 'handleSdpiItemChange' method below.
 */
function prepareDOMElements(baseElement) {
    const onchangeevt = 'onchange'; // 'oninput'; // change this, if you want interactive elements act on any change, or while they're modified

    baseElement = baseElement || document;
    Array.from(baseElement.querySelectorAll('.sdpi-item-value')).forEach(
        (el, i) => {
            const elementsToClick = [
                'BUTTON',
                'OL',
                'UL',
                'TABLE',
                'METER',
                'PROGRESS',
                'CANVAS',
                'DATALIST'
            ].includes(el.tagName);
            const evt = elementsToClick ? 'onclick' : onchangeevt || 'onchange';

            /** Look for <input><span> combinations, where we consider the span as label for the input
             * we don't use `labels` for that, because a range could have 2 labels.
             */
            const inputGroup = el.querySelectorAll('input + span');
            if (inputGroup.length === 2) {
                const offs = inputGroup[0].tagName === 'INPUT' ? 1 : 0;
                inputGroup[offs].innerText = inputGroup[1 - offs].value;
                inputGroup[1 - offs]['oninput'] = function () {
                    inputGroup[offs].innerText = inputGroup[1 - offs].value;
                };
            }
            /** We look for elements which have an 'clickable' attribute
             * we use these e.g. on an 'inputGroup' (<span><input type="range"><span>) to adjust the value of
             * the corresponding range-control
             */
            Array.from(el.querySelectorAll('.clickable')).forEach(
                (subel, subi) => {
                    subel['onclick'] = function (e) {
                        handleSdpiItemChange(e.target, subi);
                    };
                }
            );
            /** Just in case the found HTML element already has an input or change - event attached, 
             * we clone it, and call it in the callback, right before the freshly attached event
            */
            const cloneEvt = el[evt];
            el[evt] = function (e) {
                if (cloneEvt) cloneEvt();
                handleSdpiItemChange(e.target, i);
            };
        }
    );

    /**
     * You could add a 'label' to a textares, e.g. to show the number of charactes already typed
     * or contained in the textarea. This helper updates this label for you.
     */
    baseElement.querySelectorAll('textarea').forEach((e) => {
        const maxl = e.getAttribute('maxlength');
        e.targets = baseElement.querySelectorAll(`[for='${e.id}']`);
        if (e.targets.length) {
            let fn = () => {
                for (let x of e.targets) {
                    x.textContent = maxl ? `${e.value.length}/${maxl}` : `${e.value.length}`;
                }
            };
            fn();
            e.onkeyup = fn;
        }
    });

    baseElement.querySelectorAll('[data-open-url]').forEach(e => {
        const value = e.getAttribute('data-open-url');
        if (value) {
            e.onclick = () => {
                let path;
                if (value.indexOf('http') !== 0) {
                    path = document.location.href.split('/');
                    path.pop();
                    path.push(value.split('/').pop());
                    path = path.join('/');
                } else {
                    path = value;
                }
                $SD.api.openUrl($SD.uuid, path);
            };
        } else {
            console.log(`${value} is not a supported url`);
        }
    });
}

function handleSdpiItemChange(e, idx) {

    /** Following items are containers, so we won't handle clicks on them */

    if (['OL', 'UL', 'TABLE'].includes(e.tagName)) {
        return;
    }

    /** SPANS are used inside a control as 'labels'
     * If a SPAN element calls this function, it has a class of 'clickable' set and is thereby handled as
     * clickable label.
     */

    if (e.tagName === 'SPAN' || e.tagName === 'OPTION') {
        const inp = e.tagName === 'OPTION' ? e.closest('.sdpi-item-value')?.querySelector('input') : e.parentNode.querySelector('input');
        var tmpValue;

        // if there's no attribute set for the span, try to see, if there's a value in the textContent
        // and use it as value
        if (!e.hasAttribute('value')) {
            tmpValue = Number(e.textContent);
            if (typeof tmpValue === 'number' && tmpValue !== null) {
                e.setAttribute('value', 0 + tmpValue); // this is ugly, but setting a value of 0 on a span doesn't do anything
                e.value = tmpValue;
            }
        } else {
            tmpValue = Number(e.getAttribute('value'));
        }
        console.log("clicked!!!!", e, inp, tmpValue, e.closest('.sdpi-item-value'), e.closest('input'));

        if (inp && tmpValue !== undefined) {
            inp.value = tmpValue;
        } else return;
    }

    const selectedElements = [];
    const isList = ['LI', 'OL', 'UL', 'DL', 'TD'].includes(e.tagName);
    const sdpiItem = e.closest('.sdpi-item');
    const sdpiItemGroup = e.closest('.sdpi-item-group');
    let sdpiItemChildren = isList
        ? sdpiItem.querySelectorAll(e.tagName === 'LI' ? 'li' : 'td')
        : sdpiItem.querySelectorAll('.sdpi-item-child > input');

    if (isList) {
        const siv = e.closest('.sdpi-item-value');
        if (!siv.classList.contains('multi-select')) {
            for (let x of sdpiItemChildren) x.classList.remove('selected');
        }
        if (!siv.classList.contains('no-select')) {
            e.classList.toggle('selected');
        }
    }

    if (sdpiItemChildren.length && ['radio', 'checkbox'].includes(sdpiItemChildren[0].type)) {
        e.setAttribute('_value', e.checked); //'_value' has priority over .value
    }
    if (sdpiItemGroup && !sdpiItemChildren.length) {
        for (let x of ['input', 'meter', 'progress']) {
            sdpiItemChildren = sdpiItemGroup.querySelectorAll(x);
            if (sdpiItemChildren.length) break;
        }
    }

    if (e.selectedIndex !== undefined) {
        if (e.tagName === 'SELECT') {
            sdpiItemChildren.forEach((ec, i) => {
                selectedElements.push({ [ec.id]: ec.value });
            });
        }
        idx = e.selectedIndex;
    } else {
        sdpiItemChildren.forEach((ec, i) => {
            if (ec.classList.contains('selected')) {
                selectedElements.push(ec.textContent);
            }
            if (ec === e) {
                idx = i;
                selectedElements.push(ec.value);
            }
        });
    }

    const returnValue = {
        key: e.id && e.id.charAt(0) !== '_' ? e.id : sdpiItem.id,
        value: isList
            ? e.textContent
            : e.hasAttribute('_value')
                ? e.getAttribute('_value')
                : e.value
                    ? e.type === 'file'
                        ? decodeURIComponent(e.value.replace(/^C:\\fakepath\\/, ''))
                        : e.value
                    : e.getAttribute('value'),
        group: sdpiItemGroup ? sdpiItemGroup.id : false,
        index: idx,
        selection: selectedElements,
        checked: e.checked
    };

    /** Just simulate the original file-selector:
     * If there's an element of class '.sdpi-file-info'
     * show the filename there
     */
    if (e.type === 'file') {
        const info = sdpiItem.querySelector('.sdpi-file-info');
        if (info) {
            const s = returnValue.value.split('/').pop();
            info.textContent = s.length > 28
                ? s.substr(0, 10)
                + '...'
                + s.substr(s.length - 10, s.length)
                : s;
        }
    }

    sendValueToPlugin(returnValue, 'sdpi_collection');
}
function rangeToPercent(value, min, max) {
    return (value - min) / (max - min);
};

function initToolTips() {
    const tooltip = document.querySelector('.sdpi-info-label');
    const arrElements = document.querySelectorAll('.floating-tooltip');
    arrElements.forEach((e, i) => {
        initToolTip(e, tooltip);
    });
}

function initToolTip(element, tooltip) {

    const tw = tooltip.getBoundingClientRect().width;
    const suffix = element.getAttribute('data-suffix') || '';

    const fn = () => {
        const elementRect = element.getBoundingClientRect();
        const w = elementRect.width - tw / 2;
        const percnt = rangeToPercent(element.value, element.min, element.max);
        tooltip.textContent = suffix != "" ? `${element.value} ${suffix}` : String(element.value);
        tooltip.style.left = `${elementRect.left + Math.round(w * percnt) - tw / 4}px`;
        tooltip.style.top = `${elementRect.top + 20}px`;
    };

    if (element) {
        element.addEventListener('mouseenter', function () {
            tooltip.classList.remove('hidden');
            tooltip.classList.add('shown');
            fn();
        }, false);

        element.addEventListener('mouseout', function () {
            tooltip.classList.remove('shown');
            tooltip.classList.add('hidden');
            fn();
        }, false);
        element.addEventListener('input', fn, false);
    }
}
