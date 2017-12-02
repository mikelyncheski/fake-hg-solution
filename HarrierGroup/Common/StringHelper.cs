#region Copyright Notice
//
//  Copyright (c) 2017 - The Harrier Group, Inc.
//  All Rights Reserved
//
//  http://www.harriergroup.com
//
#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions; 

namespace HarrierGroup.Common
{
	//
	// Enums used in FormatTaxId function below
	//
	public enum TaxIdFormats
	{
		FederalTaxId,
		SocialSecurityNumber
	};


	public partial class StringHelper
	{
		public static string UnMike(string s) => s.Replace("Mike", "Kristyn");


		public static string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
		{
			var sb = new StringBuilder();
			var previousIndex = 0;
			var index = str.IndexOf(oldValue, comparison);

			while (index != -1)
			{
				sb.Append(str.Substring(previousIndex, index - previousIndex));
				sb.Append(newValue);
				index += oldValue.Length;

				previousIndex = index;
				index = str.IndexOf(oldValue, index, comparison);
			}
			sb.Append(str.Substring(previousIndex));

			return sb.ToString();
		}


		public static string Right(string s, int len)
		{
			if (len > s.Length)
				return s;
			return s.Substring(s.Length - len);
		}


		public static string Left(string s, int len)
		{
			if (len > s.Length)
				return s;
			return s.Substring(0, len);
		}


		public static string IntSuffix(int i)
		{
			return IntSuffix(i.ToString());
		}


		public static string IntSuffix(string num)
		{
			string suffix;

			num = Right(num, 2);

			switch (num)
			{
				case "1":
				case "21":
					suffix = "st";
					break;

				case "2":
				case "22":
					suffix = "nd";
					break;

				case "3":
				case "23":
					suffix = "rd";
					break;

				default:
					suffix = Int32.Parse(num) <= 20 ? "th" : IntSuffix(Right(num, 1));
					break;
			}

			return suffix;
		}


		// ReSharper disable once InconsistentNaming
		public static string NS(string s)
		{
			if (s == null)
				return string.Empty;

			return (s);
		}


		// ReSharper disable once InconsistentNaming
		public static string NS(string s, string defaultValue)
		{
			s = NS(s);

			if (s.Length == 0)
				return defaultValue ?? string.Empty;

			return (s);
		}


		public static string NullIf(string s, string ifValue = "")
		{
			if (s == null)
				return s;

			return s.Equals(ifValue) ? null : s;
		}


		// ReSharper disable once InconsistentNaming
		public static string ZS(int i)
		{
			if (i == 0)
				return string.Empty;

			return i.ToString();
		}


		// ReSharper disable once InconsistentNaming
		public static string ZS(int i, string defaultValue)
		{
			var s = ZS(i);

			if (s.Length == 0)
				return defaultValue;

			return (s);
		}


		//
		// Skip the ~NNNN that is found at the front of some lookup values.
		//
		public static string DimView(string s)
		{
			if (s.Length < 5)
				return s;

			if (s.StartsWith("~"))
				return s.Substring(5);

			return s;
		}


		public static string DigitsOnly(string number)
		{
			return DigitsOnly(number, string.Empty);
		}


		/// <summary>
		/// DigitsOnly returns the digits (0-9) found in the input string.
		/// </summary>
		/// <param name="number">Non-digit characters are ignored.</param>
		/// <param name="exceptions">
		/// Characters in this string will be returned in addition to any digits found.
		/// </param>
		/// <returns>
		/// The digits found in the input string, in addition to any 
		/// characters in the second param.
		/// </returns>
		public static string DigitsOnly(string number, string exceptions)
		{
			if (number == null)
				return string.Empty;

			var s = new StringBuilder();

			foreach (var c in number)
			{
				if ((c >= '0' && c <= '9') || exceptions.IndexOf(c) != -1)
					s.Append(c);
			}

			return s.ToString();
		}


		public static bool IsAllDigits(string number)
		{
			return number.Equals(DigitsOnly(number));
		}


		public static string DollarDigitsOnly(string number)
		{
			return DigitsOnly(number, "-.");
		}


		public static string DigitsAndLettersOnly(string sIn)
		{
			if (sIn == null)
				return string.Empty;

			var s = new StringBuilder();

			foreach (var ch in sIn)
				if ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9'))
					s.Append(ch);

			return s.ToString();
		}


		/// <summary>
		/// Basically, letters, digits and the underscore character.
		/// </summary>
		/// <param name="sIn"></param>
		/// <returns></returns>
		public static string UrlCharsOnly(string sIn)
		{
			if (sIn == null)
				return string.Empty;

			var s = new StringBuilder();

			foreach (var ch in sIn)
				if ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9') || ch == '_')
					s.Append(ch);

			return s.ToString();
		}


		public static string GetUriDirectory(Uri uri)
		{
			var folder = uri.LocalPath;

			// ReSharper disable once StringLastIndexOfIsCultureSpecific.1
			var index = folder.LastIndexOf("/");
			if (index >= 0)
				folder = folder.Substring(0, index);

			return folder;
		}


		public static object ParseEnum(Type enumType, string value, Enum defaultValue)
		{
			try
			{
				return Enum.Parse(enumType, value, true);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}


		public static Guid? ParseGuid(string guid)
		{
			Guid value;
			return Guid.TryParse(guid, out value) ? value : new Guid?();
		}


		/// <summary>
		/// Converts a string to an integer number without any chance of failing.
		/// It is indestructible (good for converting QueryString arguments).
		/// </summary>
		/// <param name="number">Non-digit characters are ignored.</param>
		/// <returns>The converted number.</returns>
		public static int ParseInt(string number)
		{
			return ParseInt(number, 0);
		}


		public static int ParseInt(string number, int defaultValue)
		{
			if (string.IsNullOrEmpty(number))
				return defaultValue;

			number = DigitsOnly(number, "-");
			if (number.Length == 0)
				return 0;

			var value = 0;

			try
			{
				value = Int32.Parse(number);
			}
			catch (OverflowException)
			{
			}
			catch (FormatException)
			{
			}

			return value;
		}


		/// <summary>
		/// Converts a string to an LONG integer number without any chance of failing.
		/// It is indestructible (good for converting QueryString arguments).
		/// </summary>
		/// <param name="number">Non-digit characters are ignored.</param>
		/// <returns>The converted number.</returns>
		public static long ParseLong(string number)
		{
			return ParseLong(number, 0);
		}


		public static long ParseLong(string number, long defaultValue)
		{
			if (string.IsNullOrEmpty(number))
				return defaultValue;

			number = DigitsOnly(number);
			if (number.Length == 0)
				return 0L;

			var value = 0L;

			try
			{
				value = Int64.Parse(number);
			}
			catch (OverflowException)
			{
			}
			catch (FormatException)
			{
			}

			return value;
		}


		/// <summary>
		/// Converts a string to a SHORT integer number without any chance of failing.
		/// It is indestructible (good for converting QueryString arguments).
		/// </summary>
		/// <param name="number"></param>
		/// <returns>The converted number</returns>
		public static short ParseShort(string number)
		{
			return ParseShort(number, 0);
		}


		public static short ParseShort(string number, short defaultValue)
		{
			if (number == null)
				return defaultValue;

			number = DigitsOnly(number);
			if (number.Length == 0)
				return 0;

			short value = 0;

			try
			{
				value = Int16.Parse(number);
			}
			catch (OverflowException)
			{
			}
			catch (FormatException)
			{
			}

			return value;
		}


		/// <summary>
		/// Converts a string to a double number without any chance of failing.
		/// It is indestructible (good for converting QueryString arguments).
		/// </summary>
		/// <param name="number"></param>
		/// <returns>The converted number</returns>
		public static double ParseDouble(string number)
		{
			return ParseDouble(number, 0.0);
		}


		public static double ParseDouble(string number, double defaultValue)
		{
			if (number == null)
				return defaultValue;

			number = DollarDigitsOnly(number);
			if (number.Length == 0)
				return 0;

			double value = 0;

			try
			{
				value = Double.Parse(number);
			}
			catch (OverflowException)
			{
			}
			catch (FormatException)
			{
			}

			return value;
		}


		public static bool IsDouble(string number)
		{
			number = DollarDigitsOnly(number);
			if (number.Length == 0)
				return true;

			try
			{
				// ReSharper disable once UnusedVariable
				var value = Double.Parse(number);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}


		/// <summary>
		/// Converts a string to a decimal number without any chance of failing.
		/// It is indestructible (good for converting QueryString arguments).
		/// </summary>
		/// <param name="number"></param>
		/// <returns>The converted number</returns>
		public static decimal ParseDecimal(string number)
		{
			return ParseDecimal(number, 0.0M);
		}


		public static decimal ParseDecimal(string number, decimal defaultValue)
		{
			if (number == null)
				return defaultValue;

			number = DollarDigitsOnly(number);
			if (number.Length == 0)
				return 0;

			decimal value = 0;

			try
			{
				value = decimal.Parse(number);
			}
			catch (OverflowException)
			{
			}
			catch (FormatException)
			{
			}

			return value;
		}


		public static bool IsDecimal(string number)
		{
			number = DollarDigitsOnly(number);
			if (number.Length == 0)
				return true;

			try
			{
				// ReSharper disable once UnusedVariable
				var value = decimal.Parse(number);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}


		public static bool IsEMailAddress(string eMailAddress)
		{
			if (eMailAddress.Length == 0)
				return true;

			var regex = new Regex("\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*");

			var m = regex.Match(eMailAddress);

			return m.Success;
		}


		/// <summary>
		/// Converts a string to a DateTime without any chance of failing.
		/// It is indestructible (good for converting QueryString arguments).
		/// Note that AM/PM tags are ignored!!! 9/4/2014: KJL - Sometimes - see below where we try to handle AM/PM before giving up on it
		/// </summary>
		/// <param name="s">Non-date characters are ignored.</param>
		/// <returns>The converted DateTime.</returns>
		public static DateTime ParseDateTime(string s)
		{
			return ParseDateTime(s, DateTime.MinValue);
		}


		public static DateTime ParseDateTime(string s, DateTime defaultValue)
		{
			try
			{
				// Just parse it first in the event we're given a good date & time string that doesn't need special handling
				return DateTime.Parse(s);
			}
			catch (Exception)
			{
				var value = defaultValue;

				s = DigitsOnly(s, "/-: ");
				if (s.Length == 0)
					return value;

				try
				{
					return DateTime.Parse(s);
				}
				catch (Exception)
				{
					return defaultValue;
				}
			}
		}


		//
		// Replace both the upper and lower case old string with the new one.
		//
		public static string ReplaceUpperLower(string s, string oldValue, string newValue)
		{
			s = s.Replace(oldValue.ToLower(), newValue);
			s = s.Replace(oldValue.ToUpper(), newValue);

			return s;
		}


		// http://stackoverflow.com/a/244933/33787
		public static string ReplaceAnyCase(string s, string oldValue, string newValue)
		{
			var sb = new StringBuilder();

			int previousIndex = 0;
			int index = s.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
			while (index != -1)
			{
				sb.Append(s.Substring(previousIndex, index - previousIndex));
				sb.Append(newValue);
				index += oldValue.Length;

				previousIndex = index;
				index = s.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase);
			}
			sb.Append(s.Substring(previousIndex));

			return sb.ToString();
		}


		public static string ReplaceFirst(string original, string oldPart, string newPart, int startIndex = 0)
		{
			// ReSharper disable once StringIndexOfIsCultureSpecific.2
			var index = original.IndexOf(oldPart, startIndex);
			if (index < 0)
				return original;

			return original.Remove(index, oldPart.Length).Insert(index, newPart);
		}


		public static string ReplaceChunk(string s, int index, int length, string newPart)
		{
			return s.Substring(0, index) + newPart + s.Substring(index + length);
		}


		public static string InitialCaps(string s)
		{
			return InitialCaps(s, Int32.MaxValue);
		}


		public static string InitialCaps(string s, int maxChars)
		{
			var count = 0;
			var sb = new StringBuilder();

			if (s == null)
				return string.Empty;

			var inParens = false;
			var wasSpace = true;

			foreach (var ch in s)
			{
				var newCh = ch;

				if (count++ < maxChars)
				{
					switch (ch)
					{
						case '(':
							inParens = true;
							break;

						case ')':
							inParens = false;
							break;

						case ' ':
						case '/':
						case '-':
							wasSpace = true;
							break;

						default:
							if (wasSpace)
							{
								if (!inParens)
									newCh = ch.ToString().ToUpper()[0];
								wasSpace = false;
							}
							else
							{
								if (!inParens)
									newCh = ch.ToString().ToLower()[0];
							}
							break;
					}
				}

				sb.Append(newCh);
			}

			return sb.ToString();
		}


		public static string Repeat(string padString, int count)
		{
			var paddy = new StringBuilder();

			for (var i = 0; i < count; ++i)
				paddy.Append(padString);

			return paddy.ToString();
		}


		/// <summary>
		/// Truncate or pad a string with spaces to the specified length.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string Pad(string s, int length)
		{
			return Pad(s, length, ' ');
		}


		public static string Pad(int length)
		{
			return Pad(string.Empty, length, ' ');
		}


		/// <summary>
		/// Truncate or right pad a string with the specified fill character to the specified length.
		/// The built in PadRight method does not truncate for you.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="length"></param>
		/// <param name="fill"></param>
		/// <returns></returns>
		public static string Pad(string s, int length, char fill)
		{
			if (s == null)
			{
				s = string.Empty;
			}
			else
			{
				if (s.Length > length)
				{
					return s.Substring(0, length);
				}
			}

			return s.PadRight(length, fill);
		}


		/// <summary>
		/// Truncate or right pad a string with the specified fill character to the specified length.
		/// The built in PadLeft method does not truncate for you.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="length"></param>
		/// <param name="fill"></param>
		/// <returns></returns>
		public static string PadLeft(string s, int length, char fill)
		{
			if (s == null)
			{
				s = string.Empty;
			}
			else
			{
				if (s.Length > length)
				{
					return s.Substring(0, length);
				}
			}

			return s.PadLeft(length, fill);
		}


		public static string PadLeft(string s, int length)
		{
			return PadLeft(s, length, ' ');
		}


		/// <summary>
		/// Zero fill a number.  If the formatted number is bigger than the 
		/// specified length, then return the right-most digits only.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string ZeroFill(string number, int length)
		{
			if (number.Length >= length)
				return Right(number, length);

			return Pad(string.Empty, length - number.Length, '0') + number;
		}


		public static string ZeroFill(int number, int length)
		{
			return ZeroFill(number.ToString(), length);
		}


		public static string ZeroFill(int length)
		{
			return ZeroFill(0, length);
		}


		public static bool ParseBool(string s)
		{
			return ParseBool(s, false);
		}


		public static bool ParseBool(string s, bool defaultValue)
		{
			if (s == null)
				return defaultValue;

			switch (s.ToLower())
			{
				case "true":
				case "yes":
				case "y":
				case "1":
					return true;

				case "false":
				case "no":
				case "n":
				case "0":
					return false;

				default:
					return defaultValue;
			}
		}


		public static string YesNo(bool b)
		{
			return b ? "Yes" : "No";
		}


		public static string FormatCityState(string city, string state)
		{
			city = city.Trim();
			state = state.Trim();

			if (city.Length > 0 && state.Length > 0)
				return city + ", " + state;

			return city + state;
		}


		public static string FormatZip(string zip)
		{
			var s = DigitsOnly(NS(zip));

			if (s.Length == 9)
				return Left(s, 5) + "-" + Right(s, 4);

			return s;
		}


		public static string FormatCityStateZip(string city, string state, string zip)
		{
			return FormatCityState(city, state) + " " + FormatZip(zip);
		}


		public static string FormatAddress(string address1, string address2, string address3, string city, string state, string zip)
		{
			if (!string.IsNullOrEmpty(zip) && zip.Length < 5)
				zip = PadLeft(zip, 5, '0');
			return FormatAddress(address1, address2, address3, city, state, zip, string.Empty);
		}


		public static string FormatAddress(string address1, string address2, string address3, string city, string state, string zip, string rowDelimiter)
		{
			var line = address1;

			if (address2.Length > 0)
			{
				if (line.Length > 0)
					line += ", ";
				line += address2;
			}

			if (address3.Length > 0)
			{
				if (line.Length > 0)
					line += ", ";
				line += address3;
			}

			if (city.Length > 0)
			{
				if (line.Length > 0)
					if (rowDelimiter.Length > 0)
						line += rowDelimiter;
					else
						line += ", ";
				line += FormatCityStateZip(city, state, zip);
			}

			return line;
		}


		/// <summary>
		/// Convert the string to a date and then back to a string to format it properly.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string FormatDate(string s)
		{
			return FormatDate(ParseDateTime(s));
		}


		public static string FormatDate(DateTime? d)
		{
			if (d == null || IsNullDate(d.Value))
				return string.Empty;

			return d.Value.ToShortDateString();
		}


		public static string FormatDateByUser(DateTime date, string user)
		{
			if (date.Equals(DateTime.MinValue))
				return string.Empty;

			var result = FormatDate(date);

			if (!string.IsNullOrEmpty(user))
				result += " by " + user;

			return result;
		}


		public static string FormatWeekday(DateTime date)
		{
			return date.DayOfWeek.ToString() + " " + FormatDate(date);
		}


		public static string FormatMonthYear(DateTime d)
		{
			return d.Month.ToString() + "/" + d.Year.ToString();
		}


		public static string FormatQuarter(DateTime d)
		{
			if (d.Month <= 3)
				return "Q1";

			if (d.Month <= 6)
				return "Q2";

			if (d.Month <= 9)
				return "Q3";

			return "Q4";
		}


		public static string FormatCurrency(string s)
		{
			var d = ParseDecimal(DollarDigitsOnly(s));
			return FormatCurrency(d, true);
		}


		public static string FormatCurrency(long d)
		{
			return FormatCurrency((decimal) d, false);
		}


		public static string FormatCurrency(double d)
		{
			return FormatCurrency((decimal) d, true);
		}


		public static string FormatCurrency(double d, bool showPennies)
		{
			return FormatCurrency((decimal) d, showPennies);
		}


		public static string FormatCurrency(decimal d)
		{
			return FormatCurrency(d, true);
		}


		public static string FormatCurrency(decimal d, bool showPennies)
		{
			if (d == decimal.MinValue)
				return string.Empty;

			var c = d.ToString("n"); // Do not show dollar signs

			if (!showPennies)
				c = Left(c, c.Length - 3);

			return c;
		}


		public static string FormatCurrencyFancy(decimal d, bool showPennies = true)
		{
			var insertParens = false;

			if (d == decimal.MinValue)
				return string.Empty;

			if (d < 0)
			{
				insertParens = true;
				d = d * -1;
			}

			var s = "$" + FormatCurrency(d, showPennies);

			if (insertParens)
				s = "(" + s + ")";

			return s;
		}


		public static int Cents(double amount)
		{
			// Yes, the rounding IS necessary.  Sometimes.
			return (int) Math.Round(amount * 100.0);
		}


		public static string FormatDouble(string s, short decimalPlaces)
		{
			var d = ParseDouble(DollarDigitsOnly(s));
			return FormatDouble(d, decimalPlaces);
		}


		public static string FormatDouble(double d, short decimalPlaces)
		{
			// Gets a NumberFormatInfo associated with the en-US culture.
			var nfi = new CultureInfo("en-US").NumberFormat;
			nfi.NumberDecimalDigits = decimalPlaces;

			return d.ToString("N", nfi);
		}


		public static string FormatDecimal(string s, int decimalPlaces)
		{
			var d = ParseDecimal(DollarDigitsOnly(s));
			return FormatDecimal(d, decimalPlaces);
		}


		public static string FormatDecimal(decimal d, int decimalPlaces)
		{
			// Gets a NumberFormatInfo associated with the en-US culture.
			var nfi = new CultureInfo("en-US").NumberFormat;
			nfi.NumberDecimalDigits = decimalPlaces;

			return d.ToString("N", nfi);
		}


		public static string FormatPercent(string s)
		{
			return FormatPercent(s, true);
		}


		public static string FormatPercent(string s, bool shifted)
		{
			var p = ParseDecimal(DollarDigitsOnly(s));
			return FormatPercent(p, shifted);
		}


		public static string FormatPercent(double d)
		{
			return FormatPercent((decimal) d, true);
		}


		public static string FormatPercent(double d, bool shifted)
		{
			return FormatPercent((decimal) d, shifted);
		}


		public static string FormatPercent(decimal d)
		{
			return FormatPercent(d, true);
		}


		public static string FormatPercent(decimal p, bool shifted, int numberDecimalPlaces = 2)
		{
			// Gets a NumberFormatInfo associated with the en-US culture.
			var nfi = new CultureInfo("en-US").NumberFormat;
			nfi.NumberDecimalDigits = numberDecimalPlaces;

			if (shifted)
				p *= 100;

			return p.ToString("N", nfi);
		}


		public static string FormatInteger(string s)
		{
			return FormatInteger(ParseInt(s));
		}


		public static string FormatInteger(int i)
		{
			if (i == int.MinValue)
				return string.Empty;

			// Gets a NumberFormatInfo associated with the en-US culture.
			var nfi = new CultureInfo("en-US").NumberFormat;
			nfi.NumberDecimalDigits = 0;

			return i.ToString("N", nfi);
		}


		public static string FormatFileSize(long size)
		{
			double s = size;
			var format = new[]
			{
				"{0} Bytes", "{0} KB", "{0} MB", "{0} GB"
			};
			var i = 0;
			while (i < format.Length - 1 && s >= 1024)
			{
				s = (100 * s / 1024) / 100.0;
				i++;
			}

			return string.Format(format[i], s.ToString("###,###,##0.##"));
		}


		public static string FormatTime(DateTime d)
		{
			if (IsNullDate(d))
				return string.Empty;

			return d.ToShortTimeString();
		}


		public static string FormatTime(DateTime d, char padChar)
		{
			var s = FormatTime(d);

			if (s.Length == 7)
				s = padChar + s;

			return s;
		}


		public static string FormatDateTime(DateTime? d, bool hide0000Time = false)
		{
			if (d == null || IsNullDate(d.Value))
				return string.Empty;

			var s = FormatDate(d.Value);

			if (!hide0000Time || d.Value.TimeOfDay.TotalSeconds > 0)
				s += " " + FormatTime(d.Value);

			return s;
		}


		public static bool IsNullDate(DateTime d)
		{
			// 1899 is the Sql MinDate
			return d.Equals(DateTime.MinValue) || d.Equals(DateTime.MaxValue) || d.Equals(new DateTime(1899, 12, 30)) || d.Equals(new DateTime(1800, 1, 1));
		}


		public static string FormatPhone(string phone)
		{
			var s = DigitsOnly(NS(phone));

			if (s.Length != 10)
				return phone;

			return "(" + s.Substring(0, 3) + ") " + s.Substring(3, 3) + "-" + s.Substring(6, 4);
		}


		public static string FormatPhone(string phone, string extn)
		{
			var x = DigitsOnly(NS(extn));

			if (x.Length != 0)
				return FormatPhone(phone) + " x" + x;

			return FormatPhone(phone);
		}


		public static string FormatPhone(string phone, string extn, string separator)
		{
			var s = DigitsOnly(NS(phone));

			if (s.Length != 10)
				return phone;

			return string.IsNullOrWhiteSpace(extn)
					   ? s.Substring(0, 3) + separator + s.Substring(3, 3) + separator + s.Substring(6, 4)
					   : s.Substring(0, 3) + separator + s.Substring(3, 3) + separator + s.Substring(6, 4) + " x" + extn;
		}


		public static string FormatCreditCard(string creditCard)
		{
			var cc = DigitsOnly(NS(creditCard));

			if (cc.Length == 15)
				return cc.Substring(0, 4) + "-" + cc.Substring(4, 6) + "-" + cc.Substring(10, 5);

			if (cc.Length == 16)
				return cc.Substring(0, 4) + "-" + cc.Substring(4, 4) + "-" + cc.Substring(8, 4) + "-" + cc.Substring(12, 4);

			return cc;
		}


		public static string FormatTaxId(string taxId, TaxIdFormats format)
		{
			var result = taxId;
			var s = DigitsOnly(NS(taxId));

			if (taxId.Replace("0", string.Empty).Length == 0)
				return string.Empty;

			if (s.Length != 9)
				return taxId;

			switch (format)
			{
				case TaxIdFormats.FederalTaxId:
					result = s.Substring(0, 2) + "-" + s.Substring(2, 7);
					break;

				case TaxIdFormats.SocialSecurityNumber:
					result = s.Substring(0, 3) + "-" + s.Substring(3, 2) + "-" + s.Substring(5, 4);
					break;
			}

			return result;
		}


		public static string FormatZipCode(string zipCode)
		{
			var s = DigitsOnly(NS(zipCode));

			if (s.Length == 5)
				return s;

			if (s.Length == 9)
				return Left(s, 5) + "-" + Right(s, 4);

			return zipCode;
		}


		/// <summary>
		/// Format email in "address book display name" format.  This is used by
		/// MailMessage class in the From and To properties.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="email"></param>
		/// <returns></returns>
		public static string FormatEMailWithName(string name, string email)
		{
			if (string.IsNullOrEmpty(name))
				return email;

			return name + " <" + email + ">";
		}


		/// <summary>
		/// Parse a full name into individual first/last name pieces being aware
		/// of some common suffixes.
		/// </summary>
		/// <param name="fullName"></param>
		/// <param name="firstName"></param>
		/// <param name="lastName"></param>
		public static void SplitFullName(string fullName, out string firstName, out string lastName)
		{
			fullName = CleanFullName(fullName);

			firstName = string.Empty;
			lastName = string.Empty;
			var nameArray = fullName.Split(' ');

			var startOfLastName = StartOfLastName(nameArray);

			for (var i = 0; i < startOfLastName; i++)
				firstName += nameArray[i] + " ";

			for (var i = startOfLastName; i < nameArray.Length; i++)
				lastName += nameArray[i] + " ";

			firstName = firstName.TrimEnd();
			lastName = lastName.TrimEnd();
		}


		//
		// Quick and dirty First/last name functions.
		//
		public static string FirstName(string fullName)
		{
			var nameArray = fullName.Split(' ');

			return nameArray[0];
		}


		public static string LastName(string fullName)
		{
			// ReSharper disable RedundantAssignment
			var firstName = string.Empty;
			var lastName = string.Empty;
			// ReSharper restore RedundantAssignment

			SplitFullName(fullName, out firstName, out lastName);

			return lastName;
		}


		/// <summary>
		/// Helper function for ParseName
		/// </summary>
		/// <returns></returns>
		private static int StartOfLastName(string[] nameArray)
		{
			int i;

			if (nameArray.Length == 2)
				return 1;

			for (i = nameArray.Length - 1; i >= 0; i--)
				if (!IsSuffix(nameArray[i]))
					break;

			return i;
		}


		private static bool IsSuffix(string s)
		{
			string[] suffixes =
			{
				"JR", "SR", "II", "III", "IV",
				"MD", "DDS", "DVM", "DPM", "DO", "OD", "DC", "RN",
				"ESQ", "CPA", "CFP", "PC",
				"PHD", "MBA"
			};

			return suffixes.Any(suffix => s.ToUpper().Equals(suffix));
		}


		private static string CleanFullName(string fullName)
		{
			fullName = fullName.Replace(" M.D.", " MD");
			fullName = fullName.Replace(".", " ");
			fullName = fullName.Replace(",", " ");
			fullName = fullName.Replace("111", "III");
			fullName = fullName.Replace("11", "II");
			fullName = fullName.Replace("`", "'");

			// ReSharper disable once StringIndexOfIsCultureSpecific.1
			while (fullName.IndexOf("  ") > 0)
				fullName = fullName.Replace("  ", " ");

			return fullName.Trim();
		}


		public static string ByteArrayToHexString(IEnumerable<byte> array)
		{
			return array.Aggregate(new StringBuilder(32), (sb, b) => sb.Append(b.ToString("X2"))).ToString();
		}


		/// <summary>
		/// e.g., "ProductSku" -> "product_sku"
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string CamelCaseToUnderscores(string s)
		{
			var sb = new StringBuilder();
			var lastUpper = true; // no underscore the first time

			foreach (var c in s)
			{
				if (c >= 'A' && c <= 'Z')
				{
					if (!lastUpper)
						sb.Append("_");
					lastUpper = true;
				}
				else
					lastUpper = false;

				sb.Append(c);
			}

			return sb.ToString().ToLower();
		}


		/// <summary>
		/// Capitalize the first letter and every letter following an underscore
		/// e.g., "product_sku" -> "ProductSku"
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string UnderscoresToCamelCase(string s)
		{
			var sb = new StringBuilder();
			var upperNext = true; // Uppercase the following character

			foreach (var c in s)
			{
				if (c == '_')
				{
					upperNext = true;
				}
				else if (upperNext)
				{
					sb.Append(c.ToString().ToUpper());
					upperNext = false;
				}
				else
				{
					sb.Append(c);
				}
			}

			return sb.ToString();
		}


		/// <summary>
		/// e.g., "ProductSku" -> "Product Sku"
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string CamelCaseToWords(string s)
		{
			var sb = new StringBuilder();
			var lastUpper = true; // no space the first time

			foreach (var c in s)
			{
				if (c >= 'A' && c <= 'Z')
				{
					if (!lastUpper)
						sb.Append(" ");
					lastUpper = true;
				}
				else
					lastUpper = false;

				sb.Append(c);
			}

			return sb.ToString();
		}


		/// <summary>
		/// e.g., "tbProductSku" -> "ProductSku"
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string StripLowerCasePrefix(string s)
		{
			var i = s.TakeWhile(c => c < 'A' || c > 'Z').Count();

			return s.Substring(i);
		}


		/// <summary>
		/// e.g., "tbProductSku" -> "Product Sku"
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string IdToWords(string s)
		{
			if (s.StartsWith("ddp") && s.EndsWith("Code"))
				s = Left(s, s.Length - 4);

			return CamelCaseToWords(StripLowerCasePrefix(s));
		}


		public static bool IpAddressMatch(string requestorAddress, string addressMask)
		{
			return addressMask.Split('|').Any(mask => Left(requestorAddress, mask.Trim().Length).Equals(mask.Trim()));
		}


		/// <summary>
		/// Truncate string at length and add ellipsis if too long.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string Ellipsis(string s, int length)
		{
			if (s.Length > length)
				return s.Substring(0, length) + "...";

			return s;
		}


		/// <summary>
		/// Return the last part of the user ID, i.e. the part after the domain name slash.
		/// </summary>
		/// <param name="fullUserName"></param>
		/// <returns></returns>
		public static string SplitUserName(string fullUserName)
		{
			// ReSharper disable once StringLastIndexOfIsCultureSpecific.1
			var lastSlash = fullUserName.LastIndexOf(@"\");
			if (lastSlash < 0)
				return fullUserName;

			return fullUserName.Substring(lastSlash + 1);
		}


		public static List<string> SplitAtWord(string s, int splitPoint)
		{
			var list = new List<string>();

			//
			// No need to split.
			//

			if (s.Length <= splitPoint)
			{
				list.Add(s);
				return list;
			}

			//
			// If there is a space in the first part of the string, then 
			// move the split point up to that space.
			//

			var i = Left(s, splitPoint).LastIndexOf(' ');
			if (i > 0)
				splitPoint = i;

			//
			// Get the first part.
			//

			list.Add(Left(s, splitPoint).TrimEnd());

			//
			// If the string extends past the split point, then include
			// the second piece.
			//

			if (s.Length > splitPoint)
				list.Add(s.Substring(splitPoint).TrimStart());

			return list;
		}


		/// <summary>
		/// Split a mult-line string into separate lines.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static List<string> SplitLines(string s)
		{
			return SplitLines(s, false);
		}


		/// <summary>
		/// Split a mult-line string into separate lines.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="trimBlanks">Trim trailing blanks and remove blank lines</param>
		/// <returns></returns>
		public static List<string> SplitLines(string s, bool trimBlanks)
		{
			var list = new List<string>();

			if (s.Length == 0) // Split always returns one entry even for empty strings
				return list; // so prevent getting an entry of one blank line.

			foreach (var line in s.Replace(Environment.NewLine, "\n").Split('\n'))
			{
				var line2 = line;

				if (trimBlanks)
				{
					line2 = line2.Trim();
					if (line2.Length == 0)
						continue;
				}

				list.Add(line2);
			}

			return list;
		}


		public static string TrimBlankLines(string s)
		{
			var sb = new StringBuilder();

			foreach (var line in SplitLines(s, true))
				sb.AppendLine(line);

			return sb.ToString();
		}


		public static string StripBlankLines(string s)
		{
			// http://stackoverflow.com/a/7647762/33787
			return Regex.Replace(s, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
		}


		/// <summary>
		/// Electronic Funds Transfer Routing Number validation
		/// </summary>
		/// <param name="routingNumber"></param>
		/// <returns></returns>
		public static bool IsRoutingNumber(string routingNumber)
		{
			if (DigitsOnly(routingNumber).Length != 9)
				return false;

			long sum =
				3 * ToInt(routingNumber, 1) + 7 * ToInt(routingNumber, 2) + ToInt(routingNumber, 3) +
				3 * ToInt(routingNumber, 4) + 7 * ToInt(routingNumber, 5) + ToInt(routingNumber, 6) +
				3 * ToInt(routingNumber, 7) + 7 * ToInt(routingNumber, 8) + ToInt(routingNumber, 9);

			return ((sum % 10) == 0);
		}


		private static int ToInt(string routingNumber, int i)
		{
			return ParseInt(routingNumber.Substring(i - 1, 1));
		}


		public static void TrimAll(List<string> list)
		{
			for (var i = 0; i < list.Count; ++i)
				list[i] = list[i].Trim();
		}


		public static List<string> SplitFirst(string s, char separator)
		{
			var list = new List<string>();

			var i = s.IndexOf(separator);

			if (i < 0)
			{
				list.Add(s);
			}
			else
			{
				list.Add(Left(s, i));
				list.Add(s.Substring(i + 1));
			}

			return list;
		}


		public static string Reverse(string s)
		{
			var sb = new StringBuilder();

			for (var i = s.Length - 1; i >= 0; --i)
				sb.Append(s[i]);

			return sb.ToString();
		}


		public static bool IsAscii(char c)
		{
			return c >= 32 && c <= 126;
		}


		public static string FormatException(Exception e)
		{
			return FormatException(string.Empty, e, Environment.NewLine);
		}


		public static string FormatExceptionHtml(Exception e)
		{
			return FormatException(string.Empty, e, "<br/>").Replace(Environment.NewLine, "<br />");
		}


		public static string FormatException(string where, Exception e)
		{
			return FormatException(where, e, Environment.NewLine);
		}


		public static string FormatException(string where, Exception e, string lineBreak, bool includeStackTrace = true)
		{
			if (e == null)
				return string.Empty;

			if (where.Length > 0)
				where += " ";

			var eString = string.Format("{0}Exception: {1}: {2}{3}{4}",
										where, e.GetType().FullName, e.Message,
										includeStackTrace ? (lineBreak + e.StackTrace) : string.Empty,
										lineBreak + lineBreak);

			return FormatException("Inner", e.InnerException) + eString;
		}


		public static string FormatMilitaryTime(int time)
		{
			var t = new DateTime();

			// ReSharper disable once PossibleLossOfFraction
			t = t.AddHours(time / 100);
			t = t.AddMinutes(time % 100);

			return t.ToShortTimeString();
		}


		public static string FindInList(List<string> list, string pattern)
		{
			return list.FirstOrDefault(s => s.Equals(pattern));
		}


		public static bool InList(List<string> list, string pattern)
		{
			return FindInList(list, pattern) != null;
		}


		public static string ReflectToHtmlString(object o, string linePrefix = "", bool sortByName = false)
		{
			return ReflectToString(o, linePrefix: linePrefix, sortByName: sortByName).Replace(Environment.NewLine, "<br/>");
		}


		public static string ReflectToString(object o, bool separateLines = true, string linePrefix = "", bool sortByName = false)
		{
			var sb = new StringBuilder();

			if (o == null)
				return "NULL";

			var type = o.GetType();

			if (type.IsValueType || o is string)
				return string.Format("{0} = {1}\n", type.Name, o);

			IEnumerable<PropertyInfo> propertyList = type.GetProperties().Cast<PropertyInfo>();

			if (sortByName)
				propertyList = propertyList.OrderBy(pi => pi.Name);

			foreach (var pi in propertyList)
			{
				try
				{
					object value;
					var count = int.MinValue;

					try
					{
						value = pi.GetValue(o, new object[]
						{
						});
						count = GetValueCount(value);
					}
					catch (Exception e)
					{
						value = string.Format("Error getting value ({0}).", e.GetType().Name);
					}

					sb.AppendFormat("{0}{1}.{2} = {3}{4}{5}", linePrefix, type.Name, pi.Name, value,
									count >= 0 && !(pi.PropertyType == typeof (String)) ? "--" + count.ToString() + " entries" : string.Empty,
									separateLines ? Environment.NewLine : "\t");
				}
				catch (Exception)
				{
					sb.AppendLine("Error with property");
				}
			}

			return sb.ToString();
		}


		private static int GetValueCount(object o)
		{
			try
			{
				//
				// If the value is a collection, try to get the count of the elements.
				//

				var collection = o as ICollection;
				if (collection != null)
					return collection.Count;

				//
				// Count the enumeration.
				//

				var enumerable = o as IEnumerable;
				if (enumerable != null)
					return enumerable.Cast<object>().Count();
			}
				// ReSharper disable once EmptyGeneralCatchClause
			catch (Exception)
			{
			}

			return int.MinValue;
		}


		public static string GetFileExtension(string fileName)
		{
			var index = fileName.LastIndexOf('.');
			if (index < 0)
				return string.Empty;

			return Right(fileName, fileName.Length - index - 1).ToLower();
		}


		public static string FixDbQuotes(string s)
		{
			return s.Replace("'", "''");
		}


		public static string DoubleSpace(string s, int numSpaces = 1, char spaceChar = ' ')
		{
			var sb = new StringBuilder();
			var count = 0;

			foreach (var ch in s)
			{
				sb.Append(ch);

				if (++count < s.Length)
					for (var i = 0; i < numSpaces; ++i)
						sb.Append(spaceChar);
			}

			return sb.ToString();
		}


		/// <summary>
		/// Removes the specified block of text if it exists on the end of the string.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="removeText"></param>
		/// <returns></returns>
		public static string RemoveEnd(string s, string removeText)
		{
			if (s.EndsWith(removeText))
				return s.Remove(s.Length - removeText.Length);

			return s;
		}


		public static List<string> StringToList(string s, bool trimStrings = false, string separator = ",")
		{
			var list = new List<string>(s.Split(new[] { separator }, StringSplitOptions.None));

			if (trimStrings)
				TrimAll(list);

			return list;
		}


		public static string ListToString(IEnumerable<string> list, string separator = ",", string wrapper = "")
		{
			var sb = new StringBuilder();

			foreach (var code in list)
			{
				if (sb.Length > 0)
					sb.Append(separator);

				sb.Append(wrapper + code + wrapper);
			}

			return sb.ToString();
		}


		public static string ListToString(IEnumerable<int> list)
		{
			var sb = new StringBuilder();

			foreach (var number in list)
			{
				if (sb.Length > 0)
					sb.Append(",");

				sb.Append(number.ToString());
			}

			return sb.ToString();
		}


        public static bool ListStringContains(string listString, string s)
        {
            return StringToList(listString).Contains(s);
        }


		public static string Pluralize(string s, int count)
		{
			if (count == 1)
				return s;

			return s + "s";
		}


		public static string GetInitials(string s)
		{
			return s.Split(' ').Aggregate(string.Empty, (current, word) => current + Left(word, 1));
		}


		public const string VisibleInvalidFileNameChars = "<>:\"/\\|?*";


		public static string GetInvalidFileNameChars()
		{
			var badBoys = VisibleInvalidFileNameChars;

			for (int i = 0; i <= 31; ++i)
				badBoys += (char) i;

			return badBoys;
		}


		public static string CleanseFileName(string fileName, string otherCharacters = "", string replaceWith = "")
		{
			return (GetInvalidFileNameChars() + otherCharacters).Aggregate(fileName, (current, c) => current.Replace(c.ToString(), replaceWith));
		}
	}
}