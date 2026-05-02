use energydata

select * from Emerguapp

Truncate table  Emerguapp
Create VIEW vw_EnergyReport AS
SELECT
Id,
[Date],
Block,
StartTime,
EndTime,
FrequencyHz,
ActualGeneration,
DeclaredCapacity,
ScheduledGeneration,
AgcAdjustment,
OverInjection,
UnderInjection,
TotalCharge
FROM Emerguapp;


DROP VIEW IF EXISTS vw_EnergyReport;
SELECT * FROM vw_EnergyReport;
