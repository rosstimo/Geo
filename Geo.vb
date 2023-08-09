'Earth's circumference is the distance around Earth. 
'Measured around the equator, it is 40,075.017 km (24,901.461 mi). 
'Measured around the poles, the circumference is 40,007.863 km (24,859.734 mi).

Imports System

Module Geo
  Const earthCircumferenceAtEquator As Double = 40075.017 'km
  Const earthCircumferenceAtPoles As Double = 40007.863 'km
  Const pi as Double = Math.PI
  Dim points as new List(Of (longitude$, latitude$, altitude$, R$, G$, B$))
  Dim maxPoint as (longitude$ , latitude$, altitude$, R$, G$, B$)
  Dim minPoint as (longitude$ , latitude$, altitude$, R$, G$, B$)
'define bounding box
  Dim mostWestSouthLowestPoint as (longitude$ , latitude$, altitude$, R$, G$, B$)
  Dim mostEastNorthHighestPoint as (longitude$ , latitude$, altitude$, R$, G$, B$)
 

  Sub Main(args As String())
    for each arg in args
      Console.WriteLine(arg)
    next
    ' Console.WriteLine("Hello World!")
    Console.WriteLine(CircumferenceAtLatitude(0,0))
    Console.WriteLine(kmPerDegreeLongitudeAt(0,0))
    Console.WriteLine(CircumferenceAtPoles(0))
    Console.WriteLine(kmPerDegreeLatitude(0))
    ' Console.WriteLine(Convert.Test())
    ' Convert.ReadFileX()
    ReadFile("ranchPointAbsolute.txt")
    ' WritePoints()
    GetMaxMinPoints()
    Console.WriteLine($"Max: {maxPoint.longitude} {maxPoint.latitude} {maxPoint.altitude} {maxPoint.R} {maxPoint.G} {maxPoint.B}")
    Console.WriteLine($"Min: {minPoint.longitude} {minPoint.latitude} {minPoint.altitude} {minPoint.R} {minPoint.G} {minPoint.B}")
    Console.WriteLine($"Max: {maxPoint.longitude} {maxPoint.latitude} {maxPoint.altitude} {maxPoint.R} {maxPoint.G} {maxPoint.B}")
    Console.WriteLine($"Min: {minPoint.longitude} {minPoint.latitude} {minPoint.altitude} {minPoint.R} {minPoint.G} {minPoint.B}")
    relativeDistanceBetween(mostEastNorthHighestPoint, mostWestSouthLowestPoint)
  End Sub

  'this function returns the distance in km around the earth from east to west at a given latitude and optinal altitude in km
  Function CircumferenceAtLatitude(latitude As Double, optional altitude  as Double = 0) As Double
    Dim r As Double = (earthCircumferenceAtEquator/(2*pi)) + altitude
    Dim c as Double =  2 * pi * r * Math.Cos(latitude * (pi/180)) 'latitude in radians
    Return c
  End Function 

  'this function returns the distance in km around the earth from north to south at the poles with optinal altitude in km
  Function CircumferenceAtPoles(optional altitude as Double = 0) As Double
    Dim r = earthCircumferenceAtPoles / (2 * pi)
    Return 2 * pi * (r + altitude)
  End Function

  'this function returns the km per degree of longitude at a given latitude and optional altitude
  Function kmPerDegreeLongitudeAt(latitude As Double, optional altitude as Double = 0 ) As Double
    return CircumferenceAtLatitude(latitude, altitude) / 360
  End Function

  'this function returns the km per degree of latitude at and optional altitude
  Function kmPerDegreeLatitude(optional altitude as Double = 0) as Double
    return CircumferenceAtPoles(altitude) / 360
  End Function

  Function kmFromPrimeMeridian(longitude as Double, latitude as Double, optional altitude as Double = 0) as Double
    return CDbl(longitude * kmPerDegreeLongitudeAt(latitude, altitude))
  End Function

  Function kmFromEquator(latitude as Double, optional altitude as Double = 0) as Double
    return CDbl(latitude * kmPerDegreeLatitude(altitude))
  End Function

  'this function returns the distance in meters between to longitude points with optional altitude
  Function relativeDistanceBetween( currentPoint as (longitude$ , latitude$, altitude$, R$, G$, B$), _
                                    referencePoint as (longitude$ , latitude$, altitude$, R$, G$, B$) _
                                  ) as (x$ , y$, z$, R$, G$, B$)
    Dim  relativePoint As (x$, y$, z$, R$, G$, B$)
    relativePoint.x = CStr(((cDbl(currentPoint.longitude) - CDbl(referencePoint.longitude)) * kmPerDegreeLongitudeAt(referencePoint.latitude,referencePoint.altitude))*1000)
    relativePoint.y = CStr(((CDbl(currentPoint.latitude) - CDbl(referencePoint.latitude)) * kmPerDegreeLatitude(referencePoint.altitude)) * 1000)
    relativePoint.z = CStr(CDbl(currentPoint.altitude) - CDbl(referencePoint.altitude))   
    relativePoint.R = currentPoint.R
    relativePoint.G = currentPoint.G 
    relativePoint.B = currentPoint.B

    Console.WriteLine($"{currentPoint.longitude} {currentPoint.latitude} {currentPoint.altitude}")
    Console.WriteLine($"{referencePoint.longitude} {referencePoint.latitude} {referencePoint.altitude}")
    Console.WriteLine(((cDbl(currentPoint.longitude) - CDbl(referencePoint.longitude)) * kmPerDegreeLongitudeAt(referencePoint.latitude,referencePoint.altitude))*1000)
    Console.WriteLine(((CDbl(currentPoint.latitude) - CDbl(referencePoint.latitude)) * kmPerDegreeLatitude(referencePoint.altitude)) * 1000)
    Console.WriteLine(currentPoint.altitude - referencePoint.altitude)
    return relativePoint 
  End Function

  'read the contents of the absolute latitude longitude points into a list of points
  Sub ReadFile(currentFile As String)
    Dim fileNumber As Integer = FreeFile()
    Dim temp As String 
    Dim longitude$ , latitude$, altitude$, R$, G$, B$
    Dim point = (longitude, latitude, altitude, R, G, B) 
    Dim pointData() as String

    Try
      FileOpen(fileNumber, currentFile, OpenMode.Input)
      Do Until EOF(fileNumber)
        temp = LineInput(fileNumber)
        pointData = split(temp, " ")
        point.longitude = pointData(0)
        point.latitude = pointData(1)
        point.altitude = pointData(2)
        point.R = pointData(3)
        point.G = pointData(4)
        point.B = pointData(5)
        points.add(point)
      Loop
      Catch fileNotFound As IO.FileNotFoundException
        Console.WriteLine("Sorry, that file doesn't exist")
      ' Catch badFileNAme As IO.IOException
      ' Console.WriteLine("oops")
      Catch ex As Exception
        Console.WriteLine("poops")
        Console.WriteLine(ex.Message) 
        Console.WriteLine(ex.StackTrace)
    End Try
    FileClose(fileNumber)
  End Sub
  
  Sub WritePoints()
    for each point in points
      Console.WriteLine($"{point.longitude} {point.latitude} {point.altitude} {point.R} {point.G} {point.B}")
    next
  End Sub

  'determine the max/min longitude, latitude and altitude to establish a boundry reference for reletive points in 3D space
  'TODO this should probably be west most, sout most, lowest altitude to establish reletive origin point
  Sub GetMaxMinPoints()
    Dim maxLongitude as Double = 0
    Dim minLongitude as Double = 360
    Dim maxLatitude as Double = 0
    Dim minLatitude as Double = 360
    Dim maxAltitude as Double = 0
    Dim minAltitude as Double = 1000000

    For each point in points
      If Math.Abs(CDbl(point.longitude)) > Math.Abs(maxLongitude) then
        maxLongitude = point.longitude
      End If
      If Math.Abs(CDbl(point.latitude)) > Math.Abs(maxLatitude) then
        maxLatitude = point.latitude
      End If
      If Math.Abs(CDbl(point.altitude)) > Math.Abs(maxAltitude) then
        maxAltitude = point.altitude
      End If
      If Math.Abs(CDbl(point.longitude)) < Math.Abs(minLongitude) then
        minLongitude = point.longitude
      End If
      If Math.Abs(CDbl(point.latitude)) < Math.Abs(minLatitude) then
        minLatitude = point.latitude
      End If
      If Math.Abs(CDbl(point.altitude)) < Math.Abs(minAltitude) then
        minAltitude = point.altitude
      End If
    Next
    maxPoint.longitude = CStr(maxLongitude)
    maxPoint.latitude = CStr(maxLatitude)
    maxPoint.altitude = CStr(maxAltitude)
    maxPoint.R = "255"
    maxPoint.G = "255"
    maxPoint.B = "255"
    minPoint.longitude = CStr(minLongitude)
    minPoint.latitude = CStr(minLatitude)
    minPoint.altitude = CStr(minAltitude)
    minPoint.R = "0"
    minPoint.G = "0"
    minPoint.B = "0"
    mostWestSouthLowestPoint.longitude = Cstr(maxLongitude)
    mostWestSouthLowestPoint.latitude = Cstr(minLatitude)
    mostWestSouthLowestPoint.altitude = Cstr(minAltitude)
    mostWestSouthLowestPoint.R = "0"
    mostWestSouthLowestPoint.G = "255"
    mostWestSouthLowestPoint.B = "0"
    mostEastNorthHighestPoint.longitude = Cstr(minLongitude)
    mostEastNorthHighestPoint.latitude = Cstr(maxLatitude)
    mostEastNorthHighestPoint.altitude = Cstr(maxAltitude)
    mostEastNorthHighestPoint.R = "255"
    mostEastNorthHighestPoint.G = "0"
    mostEastNorthHighestPoint.B = "0"
  End Sub
End Module
