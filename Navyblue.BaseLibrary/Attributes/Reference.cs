using System;

namespace NavyBlue.AspNetCore.Lib
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class ReferenceAttribute : Attribute, IEquatable<ReferenceAttribute>
    {
        public ReferenceAttribute(Type referenceType, string assembly)
        {
            this.ReferenceType = referenceType;
            this.Assembly = assembly;
        }

        public string Assembly { get; }

        public Type ReferenceType { get; }

        #region IEquatable<ReferenceAttribute> Members

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ReferenceAttribute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && string.Equals(this.Assembly, other.Assembly) && this.ReferenceType == other.ReferenceType;
        }

        #endregion IEquatable<ReferenceAttribute> Members

        public static bool operator !=(ReferenceAttribute left, ReferenceAttribute right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(ReferenceAttribute left, ReferenceAttribute right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Returns whether the value of the given object is equal to the current <see cref="T:NavyBlue.AspNetCore.Lib.Attributes.ReferenceAttribute" />.
        /// </summary>
        /// <returns>
        ///     true if the value of the given object is equal to that of the current; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to test the value equality of. </param>
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
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer hash code.
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