// LSP: All commands can be substituted for IPatientCommand
// OCP: New commands can be added without modifying existing code

// SRP: Single Responsibility - Add Visit Command
public class AddVisitCommand : IPatientCommand
{
    private readonly IVisitManager _visitManager;
    private readonly PatientVisit _visit;

    public AddVisitCommand(IVisitManager visitManager, PatientVisit visit)
    {
        _visitManager = visitManager;
        _visit = visit;
    }

    public void Execute()
    {
        _visitManager.AddVisitDirect(_visit);
        _visitManager.SaveData();
    }

    public void Undo()
    {
        _visitManager.RemoveVisitFromList(_visit.Id);
        _visitManager.SaveData();
    }
}

// SRP: Single Responsibility - Update Visit Command
public class UpdateVisitCommand : IPatientCommand
{
    private readonly IVisitManager _visitManager;
    private readonly PatientVisit _oldVisit;
    private readonly PatientVisit _newVisit;

    public UpdateVisitCommand(IVisitManager visitManager, PatientVisit oldVisit, PatientVisit newVisit)
    {
        _visitManager = visitManager;
        _oldVisit = new PatientVisit
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
        _newVisit = new PatientVisit
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
        _visitManager.UpdateVisitDirect(_newVisit);
        _visitManager.SaveData();
    }

    public void Undo()
    {
        _visitManager.UpdateVisitDirect(_oldVisit);
        _visitManager.SaveData();
    }
}

// SRP: Single Responsibility - Delete Visit Command
public class DeleteVisitCommand : IPatientCommand
{
    private readonly IVisitManager _visitManager;
    private readonly PatientVisit _visit;

    public DeleteVisitCommand(IVisitManager visitManager, PatientVisit visit)
    {
        _visitManager = visitManager;
        _visit = new PatientVisit
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
        _visitManager.RemoveVisitFromList(_visit.Id);
        _visitManager.SaveData();
    }

    public void Undo()
    {
        _visitManager.AddVisitDirect(_visit);
        _visitManager.SaveData();
    }
}