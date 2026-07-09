using System;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.All)]
    public sealed class ReferenceAttribute : Attribute, IEquatable<ReferenceAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAttribute"/> class.
        /// </summary>
        /// <param name="referenceType">Type of the reference.</param>
        /// <param name="assembly">The assembly.</param>
        public ReferenceAttribute(Type referenceType, string assembly)
        {
            this.ReferenceType = referenceType;
            this.Assembly = assembly;
        }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        /// <value>
        /// The assembly.
        /// </value>
        public string Assembly { get; }

        /// <summary>
        /// Gets the type of the reference.
        /// </summary>
        /// <value>
        /// The type of the reference.
        /// </value>
        public Type ReferenceType { get; }

        #region IEquatable<ReferenceAttribute> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ReferenceAttribute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && string.Equals(this.Assembly, other.Assembly) && this.ReferenceType == other.ReferenceType;
        }

        #endregion IEquatable<ReferenceAttribute> Members

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ReferenceAttribute left, ReferenceAttribute right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ReferenceAttribute left, ReferenceAttribute right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Returns whether the value of the given object is equal to the current <see cref="T:NavyBlue.AspNetCore.Lib.Attributes.ReferenceAttribute" />.
        /// </summary>
        /// <param name="obj">The object to test the value equality of.</param>
        /// <returns>
        /// true if the value of the given object is equal to that of the current; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if ((ReferenceAttribute)obj == this)
                return true;
            ReferenceAttribute referenceAttribute = (ReferenceAttribute)obj;
            if (referenceAttribute != null)
                return referenceAttribute.Assembly == this.Assembly && referenceAttribute.ReferenceType == this.ReferenceType;
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Assembly?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.ReferenceType?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}