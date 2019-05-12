using System;



//Compute sun position for a given date/time and longitude/latitude.


//Blanco-Muriel et al.: Computing the Solar Vector.Solar Energy Vol 70 No 5 pp 431-441.
//http://dx.doi.org/10.1016/S0038-092X(00)00156-0

//According to the paper, "The algorithm allows .. the true solar vector to be determined with an accuracy of 0.5
//minutes of arc for the period 1999–2015."
// Algorithm help is taken from the references https://pvcdrom.pveducation.org/SUNLIGHT/sunPSA.HTM & http://www.psa.es/sdg/sunpos.htm
public class SunPositionPSA
{
    private const double Deg2Rad = Math.PI / 180.0;
    private const double Rad2Deg = 180.0 / Math.PI;
    public double azimuth;
    public double altitude;
    public void CalculateSunPosition(DateTime dateTime, double latitude, double longitude)
    {
        dateTime = dateTime.ToUniversalTime();


        // Main variables
        double dElapsedJulianDays;
        double dDecimalHours;
        double dEclipticLongitude;
        double dEclipticObliquity;
        double dRightAscension;
        double dDeclination;

        // Auxiliary variables
        double dY;
        double dX;

        // Calculate difference in days between the current Julian Day
        // and JD 2451545.0, which is noon 1 January 2000 Universal Time

        {
            long liAux1;
            long liAux2;
            double dJulianDate;
            // Calculate time of the day in UT decimal hours
            dDecimalHours = dateTime.Hour
                    + (dateTime.Minute + dateTime.Second / 60.0) / 60.0;
            // Calculate current Julian Day
            liAux1 = (dateTime.Month + 1 - 14) / 12;
            liAux2 = (1461 * (dateTime.Year + 4800 + liAux1)) / 4
                    + (367 * (dateTime.Month + 1 - 2 - 12 * liAux1)) / 12
                    - (3 * ((dateTime.Year + 4900 + liAux1) / 100)) / 4
                    + dateTime.Month - 32075;
            dJulianDate = (liAux2) - 0.5 + dDecimalHours / 24.0;
            // Calculate difference between current Julian Day and JD 2451545.0
            dElapsedJulianDays = dJulianDate - 2451545.0;
        }

        // Calculate ecliptic coordinates (ecliptic longitude and obliquity of the
        // ecliptic in radians but without limiting the angle to be less than 2*Pi
        // (i.e., the result may be greater than 2*Pi)
        {
            double dMeanLongitude;
            double dMeanAnomaly;
            double dOmega;
            dOmega = 2.1429 - 0.0010394594 * dElapsedJulianDays;
            dMeanLongitude = 4.8950630 + 0.017202791698 * dElapsedJulianDays; // Radians
            dMeanAnomaly = 6.2400600 + 0.0172019699 * dElapsedJulianDays;
            dEclipticLongitude = dMeanLongitude + 0.03341607 * Math.Sin(dMeanAnomaly) + 0.00034894
                    * Math.Sin(2 * dMeanAnomaly) - 0.0001134 - 0.0000203 * Math.Sin(dOmega);
            dEclipticObliquity = 0.4090928 - 6.2140e-9 * dElapsedJulianDays + 0.0000396 * Math.Cos(dOmega);
        }

        // Calculate celestial coordinates ( right ascension and declination ) in radians
        // but without limiting the angle to be less than 2*Pi (i.e., the result
        // may be greater than 2*Pi)
        {
            double dSinEclipticLongitude;
            dSinEclipticLongitude = Math.Sin(dEclipticLongitude);
            dY = Math.Cos(dEclipticObliquity) * dSinEclipticLongitude;
            dX = Math.Cos(dEclipticLongitude);
            dRightAscension = Math.Atan2(dY, dX);
            if (dRightAscension < 0.0)
            {
                dRightAscension = dRightAscension + 2 * Math.PI;
            }
            dDeclination = Math.Asin(Math.Sin(dEclipticObliquity) * dSinEclipticLongitude);
        }

        // Calculate local coordinates ( azimuth and zenith angle ) in degrees
        {
            double dGreenwichMeanSiderealTime;
            double dLocalMeanSiderealTime;
            double dLatitudeInRadians;
            double dHourAngle;
            double dCosLatitude;
            double dSinLatitude;
            double dCosHourAngle;
            double dParallax;
            dGreenwichMeanSiderealTime = 6.6974243242 + 0.0657098283 * dElapsedJulianDays + dDecimalHours;
            dLocalMeanSiderealTime = (dGreenwichMeanSiderealTime * 15 + longitude) * Deg2Rad;
            dHourAngle = dLocalMeanSiderealTime - dRightAscension;
            dLatitudeInRadians = latitude * Deg2Rad;
            dCosLatitude = Math.Cos(dLatitudeInRadians);
            dSinLatitude = Math.Sin(dLatitudeInRadians);
            dCosHourAngle = Math.Cos(dHourAngle);
            double zenithAngle = (Math.Acos(dCosLatitude * dCosHourAngle * Math.Cos(dDeclination)
                    + Math.Sin(dDeclination) * dSinLatitude));
            dY = -Math.Sin(dHourAngle);
            dX = Math.Tan(dDeclination) * dCosLatitude - dSinLatitude * dCosHourAngle;
            azimuth = Math.Atan2(dY, dX);
            if (azimuth < 0.0)
            {
                azimuth = azimuth + Math.PI * 2;
            }
            azimuth = azimuth * Rad2Deg;
            // Parallax Correction
            dParallax = (6371.01 / 149597890) * Math.Sin(zenithAngle);
            zenithAngle = (zenithAngle + dParallax) * Rad2Deg;
            altitude = 90 - zenithAngle;
            Console.WriteLine(azimuth);
            Console.WriteLine(altitude);
        }

    }
}
