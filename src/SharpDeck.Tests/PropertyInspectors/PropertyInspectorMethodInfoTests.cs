namespace SharpDeck.Tests.PropertyInspectors
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using SharpDeck.Events.Received;
    using SharpDeck.PropertyInspectors;
    using SharpDeck.Tests.Mocks;

    /// <summary>
    /// Provides assertions for expected behaviour within <see cref="PropertyInspectorMethodInfo"/>.
    /// </summary>
    [TestFixture]
    public class PropertyInspectorMethodInfoTests
    {
        /// <summary>
        /// Asserts the <see cref="PropertyInspectorMethodInfo.SendToPluginEvent"/> and <see cref="PropertyInspectorMethodInfo.SendToPropertyInspectorEvent"/> are set, when none are specified in <see cref="PropertyInspectorMethodAttribute"/>.
        /// </summary>
        [Test]
        public void TestEventNames_None()
        {
            // given, when
            var attr = new PropertyInspectorMethodAttribute();
            var testCase = this.GetAnyMethodInfo(attr, out var methodInfo);

            // then
            Assert.That(testCase.SendToPluginEvent, Is.EqualTo(methodInfo.Name));
            Assert.That(testCase.SendToPropertyInspectorEvent, Is.EqualTo(methodInfo.Name));
        }

        /// <summary>
        /// Asserts the <see cref="PropertyInspectorMethodInfo.SendToPluginEvent"/> and <see cref="PropertyInspectorMethodInfo.SendToPropertyInspectorEvent"/> are set, when only <see cref="PropertyInspectorMethodAttribute.SendToPluginEvent"/> is specified.
        /// </summary>
        [Test]
        public void TestEventNames_SendToPluginEventOnly()
        {
            // given
            const string sendToPluginEvent = "toPlugin";

            // when
            var attr = new PropertyInspectorMethodAttribute(sendToPluginEvent);
            var testCase = this.GetAnyMethodInfo(attr, out var methodInfo);

            // then
            Assert.That(testCase.SendToPluginEvent, Is.EqualTo(sendToPluginEvent));
            Assert.That(testCase.SendToPropertyInspectorEvent, Is.EqualTo(sendToPluginEvent));
        }

        /// <summary>
        /// Asserts the <see cref="PropertyInspectorMethodInfo.SendToPluginEvent"/> and <see cref="PropertyInspectorMethodInfo.SendToPropertyInspectorEvent"/> are set, when both are specified in <see cref="PropertyInspectorMethodAttribute"/>.
        /// </summary>
        [Test]
        public void TestEventNames_Both()
        {
            // given
            const string sendToPluginEvent = "toPlugin";
            const string sendToPropertyInspectorEvent = "toPropertyInspector";

            // when
            var attr = new PropertyInspectorMethodAttribute(sendToPluginEvent, sendToPropertyInspectorEvent);
            var testCase = this.GetAnyMethodInfo(attr, out var methodInfo);

            // then
            Assert.That(testCase.SendToPluginEvent, Is.EqualTo(sendToPluginEvent));
            Assert.That(testCase.SendToPropertyInspectorEvent, Is.EqualTo(sendToPropertyInspectorEvent));
        }

        /// <summary>
        /// Asserts <see cref="PropertyInspectorMethodInfo.HasResult"/> is correctly set upon construction.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <returns><c>true</c> when the method has a return value; otherwise <c>false</c>.</returns>
        [TestCase(nameof(FooStreamDeckAction.PropertyInspector_AsyncResult), ExpectedResult = true)]
        [TestCase(nameof(FooStreamDeckAction.PropertyInspector_AsyncVoid), ExpectedResult = false)]
        [TestCase(nameof(FooStreamDeckAction.PropertyInspector_SyncResult), ExpectedResult = true)]
        [TestCase(nameof(FooStreamDeckAction.PropertyInspector_SyncVoid), ExpectedResult = false)]
        public bool TestHasResult(string methodName)
        {
            // given, when
            (var methodInfo, var attr) = this.GetParameters(methodName);
            var testCase = new PropertyInspectorMethodInfo(methodInfo, attr);

            // then
            return testCase.HasResult;
        }

        /// <summary>
        /// Asserts calling <see cref="PropertyInspectorMethodInfo.InvokeAsync(StreamDeckAction, ActionEventArgs{JObject})"/> invokes the correct method, returns the expected result.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <returns>The result of invoking the method, via <see cref="PropertyInspectorMethodInfo.InvokeAsync(StreamDeckAction, ActionEventArgs{JObject})"/>.</returns>
        [TestCase(nameof(FooStreamDeckAction.PropertyInspector_AsyncResult), ExpectedResult = nameof(FooStreamDeckAction.PropertyInspector_AsyncResult))]
        [TestCase(nameof(FooStreamDeckAction.PropertyInspector_AsyncVoid), ExpectedResult = null)]
        [TestCase(nameof(FooStreamDeckAction.PropertyInspector_SyncResult), ExpectedResult = nameof(FooStreamDeckAction.PropertyInspector_SyncResult))]
        [TestCase(nameof(FooStreamDeckAction.PropertyInspector_SyncVoid), ExpectedResult = null)]
        public async Task<string> TestInvoke(string methodName)
        {
            // given
            (var methodInfo, var attr) = this.GetParameters(methodName);
            var testCase = new PropertyInspectorMethodInfo(methodInfo, attr);

            // when
            var action = new FooStreamDeckAction();
            var args = new ActionEventArgs<JObject>() { Payload = JObject.FromObject(new FooPropertyInspectorPayload()) };
            var result = await testCase.InvokeAsync(action, args).ConfigureAwait(false);

            // then
            Assert.That(action.MethodCallCount[methodName], Is.EqualTo(1));
            return testCase.HasResult ? ((FooPropertyInspectorPayload)result).Source : null;
        }

        /// <summary>
        /// Gets a <see cref="PropertyInspectorMethodInfo"/> that can be used for testing.
        /// </summary>
        /// <param name="attr">The attribute information.</param>
        /// <param name="methodInfo">The method information used to construct the result.</param>
        /// <returns>The <see cref="PropertyInspectorMethodInfo"/>.</returns>
        private PropertyInspectorMethodInfo GetAnyMethodInfo(PropertyInspectorMethodAttribute attr, out MethodInfo methodInfo)
        {
            methodInfo = typeof(FooStreamDeckAction).GetMethod(nameof(FooStreamDeckAction.PropertyInspector_AsyncResult));
            return new PropertyInspectorMethodInfo(methodInfo, attr);
        }

        /// <summary>
        /// Gets the constructor parameters for <see cref="PropertyInspectorMethodInfo"/>, for the specified <paramref name="methodName"/> (in <see cref="FooStreamDeckAction"/>).
        /// </summary>
        /// <param name="methodName">The name of the method within <see cref="FooStreamDeckAction"/>.</param>
        /// <returns>The constructor parameters.</returns>
        public (MethodInfo methodInfo, PropertyInspectorMethodAttribute attr) GetParameters(string methodName)
        {
            var methodInfo = typeof(FooStreamDeckAction).GetMethod(methodName);
            return (methodInfo: methodInfo, attr: methodInfo.GetCustomAttribute<PropertyInspectorMethodAttribute>());
        }
    }
}
