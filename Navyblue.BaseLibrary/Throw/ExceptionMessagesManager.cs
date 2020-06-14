// *****************************************************************************************************************
// Project          : NavyBlue
// File             : ExceptionMessagesManager.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:50
// *****************************************************************************************************************
// <copyright file="ExceptionMessagesManager.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System.Globalization;
using System.Resources;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     Exception Message String Resource helper class
    /// </summary>
    public static class ExceptionMessagesManager
    {
        /// <summary>
        ///     The collection contains currently 1 element
        /// </summary>
        public const string CollectionContainsCurrently1Element = "CollectionContainsCurrently1Element";

        /// <summary>
        ///     The collection contains currently x elements
        /// </summary>
        public const string CollectionContainsCurrentlyXElements = "CollectionContainsCurrentlyXElements";

        /// <summary>
        ///     The collection is currently a null reference
        /// </summary>
        public const string CollectionIsCurrentlyANullReference = "CollectionIsCurrentlyANullReference";

        /// <summary>
        ///     The collection should be empty
        /// </summary>
        public const string CollectionShouldBeEmpty = "CollectionShouldBeEmpty";

        /// <summary>
        ///     The collection should contain 1 element
        /// </summary>
        public const string CollectionShouldContain1Element = "CollectionShouldContain1Element";

        /// <summary>
        ///     The collection should contain all of x
        /// </summary>
        public const string CollectionShouldContainAllOfX = "CollectionShouldContainAllOfX";

        /// <summary>
        ///     The collection should contain at least one of x
        /// </summary>
        public const string CollectionShouldContainAtLeastOneOfX = "CollectionShouldContainAtLeastOneOfX";

        /// <summary>
        ///     The collection should contain less than 1 element
        /// </summary>
        public const string CollectionShouldContainLessThan1Element = "CollectionShouldContainLessThan1Element";

        /// <summary>
        ///     The collection should contain less than x elements
        /// </summary>
        public const string CollectionShouldContainLessThanXElements = "CollectionShouldContainLessThanXElements";

        /// <summary>
        ///     The collection should contain more than 1 element
        /// </summary>
        public const string CollectionShouldContainMoreThan1Element = "CollectionShouldContainMoreThan1Element";

        /// <summary>
        ///     The collection should contain more than x elements
        /// </summary>
        public const string CollectionShouldContainMoreThanXElements = "CollectionShouldContainMoreThanXElements";

        /// <summary>
        ///     The collection should contain x
        /// </summary>
        public const string CollectionShouldContainX = "CollectionShouldContainX";

        /// <summary>
        ///     The collection should contain x elements
        /// </summary>
        public const string CollectionShouldContainXElements = "CollectionShouldContainXElements";

        /// <summary>
        ///     The collection should contain x or less elements
        /// </summary>
        public const string CollectionShouldContainXOrLessElements = "CollectionShouldContainXOrLessElements";

        /// <summary>
        ///     The collection should contain x or more elements
        /// </summary>
        public const string CollectionShouldContainXOrMoreElements = "CollectionShouldContainXOrMoreElements";

        /// <summary>
        ///     The collection should not be empty
        /// </summary>
        public const string CollectionShouldNotBeEmpty = "CollectionShouldNotBeEmpty";

        /// <summary>
        ///     The collection should not contain 1 element
        /// </summary>
        public const string CollectionShouldNotContain1Element = "CollectionShouldNotContain1Element";

        /// <summary>
        ///     The collection should not contain all of x
        /// </summary>
        public const string CollectionShouldNotContainAllOfX = "CollectionShouldNotContainAllOfX";

        /// <summary>
        ///     The collection should not contain any of x
        /// </summary>
        public const string CollectionShouldNotContainAnyOfX = "CollectionShouldNotContainAnyOfX";

        /// <summary>
        ///     The collection should not contain less than 1 element
        /// </summary>
        public const string CollectionShouldNotContainLessThan1Element = "CollectionShouldNotContainLessThan1Element";

        /// <summary>
        ///     The collection should not contain less than x elements
        /// </summary>
        public const string CollectionShouldNotContainLessThanXElements = "CollectionShouldNotContainLessThanXElements";

        /// <summary>
        ///     The collection should not contain more than 1 element
        /// </summary>
        public const string CollectionShouldNotContainMoreThan1Element = "CollectionShouldNotContainMoreThan1Element";

        /// <summary>
        ///     The collection should not contain more than x elements
        /// </summary>
        public const string CollectionShouldNotContainMoreThanXElements = "CollectionShouldNotContainMoreThanXElements";

        /// <summary>
        ///     The collection should not contain x
        /// </summary>
        public const string CollectionShouldNotContainX = "CollectionShouldNotContainX";

        /// <summary>
        ///     The collection should not contain x elements
        /// </summary>
        public const string CollectionShouldNotContainXElements = "CollectionShouldNotContainXElements";

        /// <summary>
        ///     The collection should not contain x or less elements
        /// </summary>
        public const string CollectionShouldNotContainXOrLessElements = "CollectionShouldNotContainXOrLessElements";

        /// <summary>
        ///     The collection should not contain x or more elements
        /// </summary>
        public const string CollectionShouldNotContainXOrMoreElements = "CollectionShouldNotContainXOrMoreElements";

        /// <summary>
        ///     The exception type is invalid
        /// </summary>
        public const string ExceptionTypeIsInvalid = "ExceptionTypeIsInvalid";

        /// <summary>
        ///     The lambda x should hold for value
        /// </summary>
        public const string LambdaXShouldHoldForValue = "LambdaXShouldHoldForValue";

        /// <summary>
        ///     The postcondition failed
        /// </summary>
        public const string PostconditionFailed = "PostconditionFailed";

        /// <summary>
        ///     The postcondition x failed
        /// </summary>
        public const string PostconditionXFailed = "PostconditionXFailed";

        /// <summary>
        ///     The string should be1 character long
        /// </summary>
        public const string StringShouldBe1CharacterLong = "StringShouldBe1CharacterLong";

        /// <summary>
        ///     The string should be empty
        /// </summary>
        public const string StringShouldBeEmpty = "StringShouldBeEmpty";

        /// <summary>
        ///     The string should be longer or equal to 1 character
        /// </summary>
        public const string StringShouldBeLongerOrEqualTo1Character = "StringShouldBeLongerOrEqualTo1Character";

        /// <summary>
        ///     The string should be longer or equal to x characters
        /// </summary>
        public const string StringShouldBeLongerOrEqualToXCharacters = "StringShouldBeLongerOrEqualToXCharacters";

        /// <summary>
        ///     The string should be longer than 1 character
        /// </summary>
        public const string StringShouldBeLongerThan1Character = "StringShouldBeLongerThan1Character";

        /// <summary>
        ///     The string should be longer than x characters
        /// </summary>
        public const string StringShouldBeLongerThanXCharacters = "StringShouldBeLongerThanXCharacters";

        /// <summary>
        ///     The string should be null or empty
        /// </summary>
        public const string StringShouldBeNullOrEmpty = "StringShouldBeNullOrEmpty";

        /// <summary>
        ///     The string should be null or white space
        /// </summary>
        public const string StringShouldBeNullOrWhiteSpace = "StringShouldBeNullOrWhiteSpace";

        /// <summary>
        ///     The string should be shorter or equal to 1 character
        /// </summary>
        public const string StringShouldBeShorterOrEqualTo1Character = "StringShouldBeShorterOrEqualTo1Character";

        /// <summary>
        ///     The string should be shorter or equal to x characters
        /// </summary>
        public const string StringShouldBeShorterOrEqualToXCharacters = "StringShouldBeShorterOrEqualToXCharacters";

        /// <summary>
        ///     The string should be shorter than 1 character
        /// </summary>
        public const string StringShouldBeShorterThan1Character = "StringShouldBeShorterThan1Character";

        /// <summary>
        ///     The string should be shorter than x characters
        /// </summary>
        public const string StringShouldBeShorterThanXCharacters = "StringShouldBeShorterThanXCharacters";

        /// <summary>
        ///     The string should be x characters long
        /// </summary>
        public const string StringShouldBeXCharactersLong = "StringShouldBeXCharactersLong";

        /// <summary>
        ///     The string should contain x
        /// </summary>
        public const string StringShouldContainX = "StringShouldContainX";

        /// <summary>
        ///     The string should end with x
        /// </summary>
        public const string StringShouldEndWithX = "StringShouldEndWithX";

        /// <summary>
        ///     The string should not be1 character long
        /// </summary>
        public const string StringShouldNotBe1CharacterLong = "StringShouldNotBe1CharacterLong";

        /// <summary>
        ///     The string should not be empty
        /// </summary>
        public const string StringShouldNotBeEmpty = "StringShouldNotBeEmpty";

        /// <summary>
        ///     The string should not be null or empty
        /// </summary>
        public const string StringShouldNotBeNullOrEmpty = "StringShouldNotBeNullOrEmpty";

        /// <summary>
        ///     The string should not be null or white space
        /// </summary>
        public const string StringShouldNotBeNullOrWhiteSpace = "StringShouldNotBeNullOrWhiteSpace";

        /// <summary>
        ///     The string should not be x characters long
        /// </summary>
        public const string StringShouldNotBeXCharactersLong = "StringShouldNotBeXCharactersLong";

        /// <summary>
        ///     The string should not contain x
        /// </summary>
        public const string StringShouldNotContainX = "StringShouldNotContainX";

        /// <summary>
        ///     The string should not end with x
        /// </summary>
        public const string StringShouldNotEndWithX = "StringShouldNotEndWithX";

        /// <summary>
        ///     The string should not start with x
        /// </summary>
        public const string StringShouldNotStartWithX = "StringShouldNotStartWithX";

        /// <summary>
        ///     The string should start with x
        /// </summary>
        public const string StringShouldStartWithX = "StringShouldStartWithX";

        /// <summary>
        ///     The actual value is1 character long
        /// </summary>
        public const string TheActualValueIs1CharacterLong = "TheActualValueIs1CharacterLong";

        /// <summary>
        ///     The actual value is x
        /// </summary>
        public const string TheActualValueIsX = "TheActualValueIsX";

        /// <summary>
        ///     The actual value is x characters long
        /// </summary>
        public const string TheActualValueIsXCharactersLong = "TheActualValueIsXCharactersLong";

        /// <summary>
        ///     The value should be a number
        /// </summary>
        public const string ValueShouldBeANumber = "ValueShouldBeANumber";

        /// <summary>
        ///     The value should be between x and y
        /// </summary>
        public const string ValueShouldBeBetweenXAndY = "ValueShouldBeBetweenXAndY";

        /// <summary>
        ///     The value should be equal to x
        /// </summary>
        public const string ValueShouldBeEqualToX = "ValueShouldBeEqualToX";

        /// <summary>
        ///     The value should be false
        /// </summary>
        public const string ValueShouldBeFalse = "ValueShouldBeFalse";

        /// <summary>
        ///     The value should be greater than or equal to x
        /// </summary>
        public const string ValueShouldBeGreaterThanOrEqualToX = "ValueShouldBeGreaterThanOrEqualToX";

        /// <summary>
        ///     The value should be greater than x
        /// </summary>
        public const string ValueShouldBeGreaterThanX = "ValueShouldBeGreaterThanX";

        /// <summary>
        ///     The value should be infinity
        /// </summary>
        public const string ValueShouldBeInfinity = "ValueShouldBeInfinity";

        /// <summary>
        ///     The value should be negative infinity
        /// </summary>
        public const string ValueShouldBeNegativeInfinity = "ValueShouldBeNegativeInfinity";

        /// <summary>
        ///     The value should be null
        /// </summary>
        public const string ValueShouldBeNull = "ValueShouldBeNull";

        /// <summary>
        ///     The value should be of type x
        /// </summary>
        public const string ValueShouldBeOfTypeX = "ValueShouldBeOfTypeX";

        /// <summary>
        ///     The value should be positive infinity
        /// </summary>
        public const string ValueShouldBePositiveInfinity = "ValueShouldBePositiveInfinity";

        /// <summary>
        ///     The value should be smaller than or equal to x
        /// </summary>
        public const string ValueShouldBeSmallerThanOrEqualToX = "ValueShouldBeSmallerThanOrEqualToX";

        /// <summary>
        ///     The value should be smaller than x
        /// </summary>
        public const string ValueShouldBeSmallerThanX = "ValueShouldBeSmallerThanX";

        /// <summary>
        ///     The value should be true
        /// </summary>
        public const string ValueShouldBeTrue = "ValueShouldBeTrue";

        /// <summary>
        ///     The value should be unequal to x
        /// </summary>
        public const string ValueShouldBeUnequalToX = "ValueShouldBeUnequalToX";

        /// <summary>
        ///     The value should be valid
        /// </summary>
        public const string ValueShouldBeValid = "ValueShouldBeValid";

        /// <summary>
        ///     The value should not be a number
        /// </summary>
        public const string ValueShouldNotBeANumber = "ValueShouldNotBeANumber";

        /// <summary>
        ///     The value should not be between x and y
        /// </summary>
        public const string ValueShouldNotBeBetweenXAndY = "ValueShouldNotBeBetweenXAndY";

        /// <summary>
        ///     The value should not be greater than or equal to x
        /// </summary>
        public const string ValueShouldNotBeGreaterThanOrEqualToX = "ValueShouldNotBeGreaterThanOrEqualToX";

        /// <summary>
        ///     The value should not be greater than x
        /// </summary>
        public const string ValueShouldNotBeGreaterThanX = "ValueShouldNotBeGreaterThanX";

        /// <summary>
        ///     The value should not be infinity
        /// </summary>
        public const string ValueShouldNotBeInfinity = "ValueShouldNotBeInfinity";

        /// <summary>
        ///     The value should not be negative infinity
        /// </summary>
        public const string ValueShouldNotBeNegativeInfinity = "ValueShouldNotBeNegativeInfinity";

        /// <summary>
        ///     The value should not be null
        /// </summary>
        public const string ValueShouldNotBeNull = "ValueShouldNotBeNull";

        /// <summary>
        ///     The value should not be of type x
        /// </summary>
        public const string ValueShouldNotBeOfTypeX = "ValueShouldNotBeOfTypeX";

        /// <summary>
        ///     The value should not be positive infinity
        /// </summary>
        public const string ValueShouldNotBePositiveInfinity = "ValueShouldNotBePositiveInfinity";

        /// <summary>
        ///     The value should not be smaller than or equal to x
        /// </summary>
        public const string ValueShouldNotBeSmallerThanOrEqualToX = "ValueShouldNotBeSmallerThanOrEqualToX";

        /// <summary>
        ///     The value should not be smaller than x
        /// </summary>
        public const string ValueShouldNotBeSmallerThanX = "ValueShouldNotBeSmallerThanX";

        /// <summary>
        ///     The resource
        /// </summary>
        private static readonly ResourceManager Resource =
            new ResourceManager(typeof(ExceptionMessagesManager).Namespace + ".Properties.Resources", typeof(ExceptionMessagesManager).Assembly);

        /// <summary>
        ///     Returns a string from the resource.
        /// </summary>
        /// <param name="name">The resource name.</param>
        /// <returns>The resource string.</returns>
        public static string GetString(string name)
        {
            return GetStringInternal(name, null);
        }

        // Returns a string from the resource and formats it with the given args in a culture-specific way.
        /// <summary>
        ///     Returns a string from the resource and formats it with the given args in a culture-specific way.
        /// </summary>
        /// <param name="name">The resource name.</param>
        /// <param name="args">The fomating arguments.</param>
        /// <returns>The formated resource string.</returns>
        public static string GetString(string name, params object[] args)
        {
            return GetStringInternal(name, args);
        }

        private static string GetStringInternal(string name, params object[] args)
        {
            // GetString will throw an ArgumentNullException when name is null.
            string format = Resource.GetString(name, CultureInfo.CurrentUICulture);
            // ReSharper disable once AssignNullToNotNullAttribute
            return args == null ? format : string.Format(CultureInfo.CurrentCulture, format, args);
        }
    }
}