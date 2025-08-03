using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public enum UserRole
{
    Admin,
    Receptionist
}

public class PatientVisit
{
    public int Id;
    public string PatientName;
    public DateTime VisitDate;
    public string VisitType;
    public string Note;
    public string DoctorName;
    public int DurationInMinutes;
    public decimal Fee;
}

public interface PatientCommand
{
    void Execute();
    void Undo();
}

public class AddVisitCommand : PatientCommand
{
    private VisitManager visitManager;
    private PatientVisit patientvisitobj;

    public AddVisitCommand(VisitManager visitManager, PatientVisit patientvisitobj)
    {
        this.visitManager = visitManager;
        this.patientvisitobj = patientvisitobj;
    }
    public void Execute()
    {
        visitManager.AddVisit_nosave(patientvisitobj);
        visitManager.saveDataPublic();
    }
    public void Undo()
    {
        visitManager.RemoveVisitFromList(patientvisitobj.Id);
        visitManager.saveDataPublic();
    }
}

public class UpdateVisitCommand : PatientCommand
{
    private VisitManager manager;
    private PatientVisit oldVisit;
    private PatientVisit newVisit;

    public UpdateVisitCommand(VisitManager manager, PatientVisit oldVisit, PatientVisit newVisit)
    {
        this.manager = manager;
        this.oldVisit = new PatientVisit
        {
            Id = oldVisit.Id,
            PatientName = oldVisit.PatientName,
            VisitDate = oldVisit.VisitDate,
            VisitType = oldVisit.VisitType,
            Note = oldVisit.Note,
            DoctorName = oldVisit.DoctorName,
            DurationInMinutes = oldVisit.DurationInMinutes,
            Fee = oldVisit.Fee
        };
        this.newVisit = new PatientVisit
        {
            Id = newVisit.Id,
            PatientName = newVisit.PatientName,
            VisitDate = newVisit.VisitDate,
            VisitType = newVisit.VisitType,
            Note = newVisit.Note,
            DoctorName = newVisit.DoctorName,
            DurationInMinutes = newVisit.DurationInMinutes,
            Fee = newVisit.Fee
        };
    }

    public void Execute()
    {
        manager.UpdateVisitDirect(newVisit);
        manager.saveDataPublic();
    }

    public void Undo()
    {
        manager.UpdateVisitDirect(oldVisit);
        manager.saveDataPublic();
    }
}

public class DeleteVisitCommand : PatientCommand
{
    private VisitManager manager;
    private PatientVisit visit;

    public DeleteVisitCommand(VisitManager manager, PatientVisit visit)
    {
        this.manager = manager;
        this.visit = new PatientVisit
        {
            Id = visit.Id,
            PatientName = visit.PatientName,
            VisitDate = visit.VisitDate,
            VisitType = visit.VisitType,
            Note = visit.Note,
            DoctorName = visit.DoctorName,
            DurationInMinutes = visit.DurationInMinutes,
            Fee = visit.Fee
        };
    }

    public void Execute()
    {
        manager.RemoveVisitFromList(visit.Id);
        manager.saveDataPublic();
    }

    public void Undo()
    {
        manager.AddVisit_nosave(visit);
        manager.saveDataPublic();
    }
}