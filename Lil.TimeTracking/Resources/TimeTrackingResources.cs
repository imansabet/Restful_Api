﻿namespace Lil.TimeTracking.Resources;

public record Employee (int Id , string Name,DateOnly StartDate);
public record Project(int Id , string Name,DateTime StartDate,DateTime? EndDate);

public record TimeEntry(Guid Id , int EmployeeId,int ProjectId
    ,DateOnly DateWorked , decimal HourseWorked);

public record ProjectAssignment(int EmployeeId,int ProjectId
    ,string? EmployeeName,string? ProjectName);

