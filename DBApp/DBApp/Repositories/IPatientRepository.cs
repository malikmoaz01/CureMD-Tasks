using DBApp.Models;

namespace DBApp.Repositories
{
    public interface IPatientRepository
    {
            Task<Patient> AddPatient(Patient patient);
            Task<Patient> UpdatePatient(Patient patient , string name);

            Task<Patient> DeletePatient(Patient patient , string name);

            Task<Patient> SearchPatient(string name , Patient patient );

            Task<Patient> GetPatient(Patient patient );
        
    }
}
