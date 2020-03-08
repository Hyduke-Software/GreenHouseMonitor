# GreenHouseMonitor
Using an Arduino and sensors logs values into a MS SQL database via a C#. Temperature, humidity and light brightness
You need a Microsoft SQL Server, with a table containing these:

Name             data type      notes
Timestamp        datetime       PRIMARY KEY + Default value or binding: (Getdate()) to auto poulate
sunlight         float
temperature      float
humidity         int
temperaturetwo   foat
