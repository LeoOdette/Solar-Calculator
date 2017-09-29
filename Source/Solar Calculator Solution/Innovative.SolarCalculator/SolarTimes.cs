﻿// ***
// *** Copyright (C) 2013-2017, Daniel M. Porrey.  All rights reserved.
// *** Written By Daniel M. Porrey
// ***
// *** This software is provided "AS IS," without a warranty of any kind. ALL EXPRESS OR IMPLIED CONDITIONS, REPRESENTATIONS AND WARRANTIES, 
// *** INCLUDING ANY IMPLIED WARRANTY OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE OR NON-INFRINGEMENT, ARE HEREBY EXCLUDED. DANIEL M PORREY 
// *** AND ITS LICENSORS SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A RESULT OF USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE 
// *** OR ITS DERIVATIVES. IN NO EVENT WILL DANIEL M PORREY OR ITS LICENSORS BE LIABLE FOR ANY LOST REVENUE, PROFIT OR DATA, OR FOR DIRECT, INDIRECT, 
// *** SPECIAL, CONSEQUENTIAL, INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND REGARDLESS OF THE THEORY OF LIABILITY, ARISING OUT OF THE USE OF 
// *** OR INABILITY TO USE THIS SOFTWARE, EVEN IF DANIEL M PORREY HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES. 
// ***
// *** Licensed under Microsoft Reciprocal License (Ms-RL)
// *** This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, 
// *** do not use the software. Full license details can be found at https://raw.githubusercontent.com/porrey/Solar-Calculator/master/LICENSE.
// ***
using System;
using Innovative.Geometry;

namespace Innovative.SolarCalculator
{
	/// <summary>
	/// Provides mathematical operations to calculate the sunrise and sunset for a given date.
	/// </summary>
	public class SolarTimes
	{
		private DateTimeOffset _forDate = DateTimeOffset.MinValue;
		private Angle _atmosphericRefraction = new Angle(0.833d);
		private Angle _longitude = Angle.Empty;
		private Angle _latitude = Angle.Empty;

		#region Constructors
		/// <summary>
		/// Creates a default instance of the SolarTimes object.
		/// </summary>
		public SolarTimes()
		{
			this.ForDate = DateTime.Now;
		}

		/// <summary>
		/// Creates an instance of the SolarTimes object with the specified For Date.
		/// </summary>
		/// <param name="forDate">Specifies the Date for which the sunrise and sunset will be calculated.</param>
		public SolarTimes(DateTimeOffset forDate)
		{
			this.ForDate = forDate;
		}

		/// <summary>
		/// Creates an instance of the SolarTimes object with the specified For Date, Latitude and Longitude.
		/// </summary>
		/// <param name="forDate">Specifies the Date for which the sunrise and sunset will be calculated.</param>
		/// <param name="latitude">Specifies the angular measurement of north-south location on Earth's surface.</param>
		/// <param name="longitude">Specifies the angular measurement of east-west location on Earth's surface.</param>
		public SolarTimes(DateTimeOffset forDate, Angle latitude, Angle longitude)
		{
			this.ForDate = forDate;
			this.Latitude = latitude;
			this.Longitude = longitude;
		}

		/// <summary>
		/// Creates an instance of the SolarTimes object with the specified For Date, Latitude Longitude and Time Zone Offset
		/// </summary>
		/// <param name="forDate">Specifies the Date for which the sunrise and sunset will be calculated.</param>
		/// <param name="timeZoneOffset">Specifies the time zone offset for the specified date in hours.</param>
		/// <param name="latitude">Specifies the angular measurement of north-south location on Earth's surface.</param>
		/// <param name="longitude">Specifies the angular measurement of east-west location on Earth's surface.</param>
		public SolarTimes(DateTime forDate, int timeZoneOffset, Angle latitude, Angle longitude)
		{
			this.ForDate = new DateTimeOffset(forDate, TimeSpan.FromHours(timeZoneOffset));
			this.Latitude = latitude;
			this.Longitude = longitude;
		}
		#endregion

		#region Public Members
		/// <summary>
		/// Specifies the Date for which the sunrise and sunset will be calculated.
		/// </summary>
		public DateTimeOffset ForDate
		{
			get
			{
				return _forDate;
			}
			set
			{
				_forDate = value;
			}
		}

		/// <summary>
		/// Angular measurement of east-west location on Earth's surface. Longitude is defined from the 
		/// prime meridian, which passes through Greenwich, England. The international date line is defined 
		/// around +/- 180° longitude. (180° east longitude is the same as 180° west longitude, because 
		/// there are 360° in a circle.) Many astronomers define east longitude as positive. This 
		/// solar calculator conforms to the international standard, with east longitude positive.
		/// (Spreadsheet Column B, Row 4)
		/// </summary>
		public Angle Longitude
		{
			get
			{
				return _longitude;
			}
			set
			{
				if (value >= -180M && value <= 180M)
				{
					_longitude = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("The value for Longitude must be between -180° and 180°.");
				}
			}
		}

		/// <summary>
		/// Angular measurement of north-south location on Earth's surface. Latitude ranges from 90° 
		/// south (at the south pole; specified by a negative angle), through 0° (all along the equator), 
		/// to 90° north (at the north pole). Latitude is usually defined as a positive value in the 
		/// northern hemisphere and a negative value in the southern hemisphere.
		/// (Spreadsheet Column B, Row 3)
		/// </summary>
		public Angle Latitude
		{
			get
			{
				return _latitude;
			}
			set
			{
				if (value >= -90M && value <= 90M)
				{
					_latitude = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("The value for Latitude must be between -90° and 90°.");
				}
			}
		}

		/// <summary>
		/// Gets the time zone offset for the specified date.
		/// Time Zones are longitudinally defined regions on the Earth that keep a common time. A time 
		/// zone generally spans 15° of longitude, and is defined by its offset (in hours) from UTC. 
		/// For example, Mountain Standard Time (MST) in the US is 7 hours behind UTC (MST = UTC - 7).
		/// (Spreadsheet Column B, Row 5)
		/// </summary>
		public decimal TimeZoneOffset
		{
			get
			{
				return (decimal)this.ForDate.Offset.TotalHours;
			}
		}

		/// <summary>
		/// Sun Rise Time  
		/// (Spreadsheet Column Y)
		/// </summary>
		public DateTime Sunrise
		{
			get
			{
				DateTime returnValue = DateTime.MinValue;

				decimal dayFraction = (decimal)this.SolarNoon.TimeOfDay.TotalDays - this.HourAngleSunrise * 4M / 1440M;
				returnValue = this.ForDate.Date.Add(DecimalTimeSpan.FromDays(dayFraction));

				return returnValue;
			}
		}

		/// <summary>
		/// Sun Set Time
		/// (Spreadsheet Column Z)
		/// </summary>
		public DateTime Sunset
		{
			get
			{
				DateTime returnValue = DateTime.MinValue;

				decimal dayFraction = (decimal)this.SolarNoon.TimeOfDay.TotalDays + this.HourAngleSunrise * 4M / 1440M;
				returnValue = this.ForDate.Date.Add(DecimalTimeSpan.FromDays(dayFraction));

				return returnValue;
			}
		}

		/// <summary>
		/// As light from the sun (or another celestial body) travels from the vacuum of space into Earth's atmosphere, the 
		/// path of the light is bent due to refraction. This causes stars and planets near the horizon to appear higher in 
		/// the sky than they actually are, and explains how the sun can still be visible after it has physically passed 
		/// beyond the horizon at sunset. See also apparent sunrise.
		/// </summary>
		public Angle AtmosphericRefraction
		{
			get
			{
				return _atmosphericRefraction;
			}
			set
			{
				_atmosphericRefraction = value;
			}
		}
		#endregion

		#region Computational Members
		/// <summary>
		/// Time past local midnight.
		/// (Spreadsheet Column E)
		/// </summary>	
		public decimal TimePastLocalMidnight
		{
			get
			{
				decimal returnValue = 0M;

				// ***
				// *** .1 / 24
				// ***
				returnValue = DateTime.Parse("12/30/1899  12:00:00 AM").Add(this.ForDate.TimeOfDay).ToOleAutomationDate();

				return returnValue;
			}
		}

		/// <summary>
		/// Julian Day: a time period used in astronomical circles, defined as the number of days 
		/// since 1 January, 4713 BCE (Before Common Era), with the first day defined as Julian 
		/// day zero. The Julian day begins at noon UTC. Some scientists use the term Julian day 
		/// to mean the numerical day of the current year, where January 1 is defined as day 001. 
		/// (Spreadsheet Column F)
		/// </summary>
		public decimal JulianDay
		{
			get
			{
				decimal returnValue = 0M;

				// ***
				// *** this.TimePastLocalMidnight was removed since the time is in ForDate
				// ***
				returnValue = ExcelFormulae.ToExcelDateValue(this.ForDate.Date) + 2415018.5M - (this.TimeZoneOffset / 24M);

				return returnValue;
			}
		}

		/// <summary>
		/// Julian Century
		/// calendar established by Julius Caesar in 46 BC, setting the number of days in a year at 365, 
		/// except for leap years which have 366, and occurred every 4 years. This calendar was reformed 
		/// by Pope Gregory XIII into the Gregorian calendar, which further refined leap years and corrected 
		/// for past errors by skipping 10 days in October of 1582. 
		/// (Spreadsheet Column G)
		/// </summary>
		public decimal JulianCentury
		{
			get
			{
				decimal returnValue = 0M;

				returnValue = (this.JulianDay - 2451545M) / 36525M;

				return returnValue;
			}
		}

		/// <summary>
		/// Sun's Geometric Mean Longitude (degrees): Geometric Mean Ecliptic Longitude of Sun.
		/// (Spreadsheet Column I)
		/// </summary>
		public Angle SunGeometricMeanLongitude
		{
			get
			{
				Angle returnValue = Angle.Empty;

				returnValue = new Angle(ExcelFormulae.Mod(280.46646M + this.JulianCentury * (36000.76983M + this.JulianCentury * 0.0003032M), 360M));

				return returnValue;
			}
		}

		/// <summary>
		/// Sun's Mean Anomaly (degrees): Position of Sun relative to perigee
		/// (Spreadsheet Column J)
		/// </summary>
		public Angle SunMeanAnomaly
		{
			get
			{
				Angle returnValue = Angle.Empty;

				returnValue = new Angle(357.52911M + this.JulianCentury * (35999.05029M - 0.0001537M * this.JulianCentury));

				return returnValue;
			}
		}

		/// <summary>
		/// Eccentricity of the Earth's Orbit: Eccentricity e is the ratio of half the distance between the foci c to
		/// the semi-major axis a: e = c / a. For example, an orbit with e = 0 is circular, e = 1 is parabolic, and e 
		/// between 0 and 1 is elliptic.
		/// (Spreadsheet Column K)
		/// </summary>
		public decimal EccentricityOfEarthOrbit
		{
			get
			{
				decimal returnValue = 0M;

				returnValue = 0.016708634M - this.JulianCentury * (0.000042037M + 0.0000001267M * this.JulianCentury);

				return returnValue;
			}
		}

		/// <summary>
		/// Sun Equation of the Center: Difference between mean anomaly and true anomaly.
		/// (Spreadsheet Column L)
		/// </summary>
		public Angle SunEquationOfCenter
		{
			get
			{
				Angle returnValue = Angle.Empty;

				returnValue = new Angle(Universal.Math.Sin(this.SunMeanAnomaly.Radians) * (1.914602M - this.JulianCentury * (0.004817M + 0.000014M * this.JulianCentury)) + Universal.Math.Sin(this.SunMeanAnomaly.RadiansMultiplied(2M)) * (0.019993M - 0.000101M * JulianCentury) + Universal.Math.Sin(this.SunMeanAnomaly.RadiansMultiplied(3M)) * 0.000289M);

				return returnValue;
			}
		}

		/// <summary>
		/// Sun True Longitude (degrees)
		/// (Spreadsheet Column M)
		/// </summary>
		public Angle SunTrueLongitude
		{
			get
			{
				Angle returnValue = Angle.Empty;

				returnValue = this.SunGeometricMeanLongitude + this.SunEquationOfCenter;

				return returnValue;
			}
		}

		/// <summary>
		/// Sun Apparent Longitude (degrees)
		/// (Spreadsheet Column P)
		/// </summary>
		public Angle SunApparentLongitude
		{
			get
			{
				Angle returnValue = Angle.Empty;

				Angle a1 = new Angle(125.04M - 1934.136M * this.JulianCentury);
				returnValue = new Angle((decimal)(this.SunTrueLongitude - 0.00569M - 0.00478M * Universal.Math.Sin(a1.Radians)));

				return returnValue;
			}
		}

		/// <summary>
		/// Mean Ecliptic Obliquity (degrees): Inclination of ecliptic plane w.r.t. celestial equator
		/// (Spreadsheet Column Q)
		/// </summary>
		public Angle MeanEclipticObliquity
		{
			get
			{
				Angle returnValue = 0d;

				// ***
				// *** Formula 22.3 from Page 147 of Astronomical Algorithms, Second Edition (Jean Meeus)
				// *** Original spreadsheet formula based on 22.2 same page of book
				// ***
				returnValue = 23M + (26M + ((21.448M - this.JulianCentury * (46.815M + this.JulianCentury * (0.00059M - this.JulianCentury * 0.001813M)))) / 60M) / 60M;

				return returnValue;
			}
		}

		/// <summary>
		/// Obliquity Correction (degrees)
		/// (Spreadsheet Column R)
		/// </summary>
		public Angle ObliquityCorrection
		{
			get
			{
				Angle returnValue = Angle.Empty;

				Angle a1 = 125.04M - 1934.136M * this.JulianCentury;
				returnValue = new Angle((decimal)(this.MeanEclipticObliquity + 0.00256M * Universal.Math.Cos(a1.Radians)));

				return returnValue;
			}
		}

		/// <summary>
		/// Solar Declination (Degrees): The measure of how many degrees North (positive) or South (negative) 
		/// of the equator that the sun is when viewed from the centre of the earth.  This varies from 
		/// approximately +23.5 (North) in June to -23.5 (South) in December.
		/// (Spreadsheet Column T)
		/// </summary>
		public Angle SolarDeclination
		{
			get
			{
				decimal returnValue = 0M;

				decimal radians = Universal.Math.Asin(Universal.Math.Sin(this.ObliquityCorrection.Radians) * Universal.Math.Sin(this.SunApparentLongitude.Radians));
				returnValue = Angle.FromRadians(radians);

				return returnValue;
			}
		}

		/// <summary>
		/// Var Y
		/// (Spreadsheet Column U)
		/// </summary>
		public decimal VarY
		{
			get
			{
				decimal returnValue = 0M;

				returnValue = Universal.Math.Tan(this.ObliquityCorrection.RadiansDivided(2M)) * Universal.Math.Tan(this.ObliquityCorrection.RadiansDivided(2M));

				return returnValue;
			}
		}

		/// <summary>
		/// Equation of Time (minutes)
		/// Accounts for changes in the time of solar noon for a given location over the course of a year. Earth's 
		/// elliptical orbit and Kepler's law of equal areas in equal times are the culprits behind this phenomenon.
		/// (Spreadsheet Column V)
		/// </summary>
		public decimal EquationOfTime
		{
			get
			{
				decimal returnValue = 0M;

				returnValue = 4M * Angle.ToDegrees(this.VarY * Universal.Math.Sin(2M * this.SunGeometricMeanLongitude.Radians) - 2M * this.EccentricityOfEarthOrbit * Universal.Math.Sin(this.SunMeanAnomaly.Radians) + 4M * this.EccentricityOfEarthOrbit * this.VarY * Universal.Math.Sin(this.SunMeanAnomaly.Radians) * Universal.Math.Cos(2M * this.SunGeometricMeanLongitude.Radians) - 0.5M * this.VarY * this.VarY * Universal.Math.Sin(4M * this.SunGeometricMeanLongitude.Radians) - 1.25M * this.EccentricityOfEarthOrbit * this.EccentricityOfEarthOrbit * Universal.Math.Sin(this.SunMeanAnomaly.RadiansMultiplied(2M)));

				return returnValue;
			}
		}

		/// <summary>
		/// HA Sunrise (degrees)
		/// (Spreadsheet Column W)
		/// </summary>
		public Angle HourAngleSunrise
		{
			get
			{
				decimal returnValue = 0M;

				Angle a1 = 90d + this.AtmosphericRefraction;
				decimal radians = Universal.Math.Acos(Universal.Math.Cos(a1.Radians) / (Universal.Math.Cos(this.Latitude.Radians) * Universal.Math.Cos(this.SolarDeclination.Radians)) - Universal.Math.Tan(this.Latitude.Radians) * Universal.Math.Tan(this.SolarDeclination.Radians));
				returnValue = Angle.FromRadians(radians);

				return returnValue;
			}
		}

		/// <summary>
		/// Solar Noon LST
		/// Defined for a given day for a specific longitude, it is the time when the sun crosses the meridian of 
		/// the observer's location. At solar noon, a shadow cast by a vertical pole will point either directly 
		/// north or directly south, depending on the observer's latitude and the time of year. 
		/// (Spreadsheet Column X)
		/// </summary>
		public DateTime SolarNoon
		{
			get
			{
				DateTime returnValue = DateTime.Now.Date;

				decimal dayFraction = (720M - (4M * this.Longitude) - this.EquationOfTime + (this.TimeZoneOffset * 60M)) / 1440M;
				returnValue = DateTime.Now.Date.Add(DecimalTimeSpan.FromDays(dayFraction));

				return returnValue;
			}
		}

		/// <summary>
		/// Sunlight Duration: The amount of time the sun is visible during the specified day.
		/// (Spreadsheet Column AA)
		/// </summary>
		public TimeSpan SunlightDuration
		{
			get
			{
				TimeSpan returnValue = TimeSpan.Zero;

				returnValue = DecimalTimeSpan.FromMinutes(8M * this.HourAngleSunrise);

				return returnValue;
			}
		}
		#endregion

		#region Obsolete Members
		/// <summary>
		/// This method is obsolete. Use Angle.ToRadians() instead.
		/// </summary>
		/// <param name="degrees">N/A</param>
		/// <returns>N/A</returns>
		[Obsolete("Use Angle.ToRadians() instead.", false)]
		public decimal ToRadians(decimal degrees)
		{
			return Angle.ToRadians(degrees);
		}

		/// <summary>
		/// This method is obsolete. Use Angle.ToDegrees() instead.
		/// </summary>
		/// <param name="radians">N/A</param>
		/// <returns>N/A</returns>
		[Obsolete("Use Angle.ToDegrees() instead.", false)]
		public decimal ToDegrees(decimal radians)
		{
			return Angle.ToDegrees(radians);
		}
		#endregion

		#region Additional Members
		/// <summary>
		/// Gets the True Solar Time in minutes. The Solar Time is defined as the time elapsed since the most recent 
		/// meridian passage of the sun. This system is based on the rotation of the Earth with respect to the sun. 
		/// A mean solar day is defined as the time between one solar noon and the next, averaged over the year.
		/// (Spreadsheet Column AB)
		/// </summary>
		public decimal TrueSolarTime
		{
			get
			{
				decimal returnValue = 0M;

				returnValue = ExcelFormulae.Mod(this.TimePastLocalMidnight * 1440M + this.EquationOfTime + 4M * this.Longitude - 60M * this.TimeZoneOffset, 1440M);

				return returnValue;
			}
		}
		#endregion
	}
}
