namespace SharpDeck.Serialization
{
    using System;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Defines a contract resolver whereby a <see cref="MemberInfo"/> must fulfil a predicate to determine whether it should be included when serialized.
    /// </summary>
    public class PredicateInclusiveContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateInclusiveContractResolver"/> class.
        /// </summary>
        /// <param name="predicate">The predicate to determine if a member should be include; when <c>true</c> the member will be serialized; otherwise it will be ignored.</param>
        public PredicateInclusiveContractResolver(Func<MemberInfo, bool> predicate)
        {
            this.CanInclude = predicate;
        }

        /// <summary>
        /// Gets the delegate that determines whether a member can be included when serializing.
        /// </summary>
        private Func<MemberInfo, bool> CanInclude { get; }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.
        /// </summary>
        /// <param name="member">The member to create a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for.</param>
        /// <param name="memberSerialization">The member's parent <see cref="T:Newtonsoft.Json.MemberSerialization" />.</param>
        /// <returns>A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (!this.CanInclude(member))
            {
                property.Ignored = true;
            }

            return property;
        }
    }
}
