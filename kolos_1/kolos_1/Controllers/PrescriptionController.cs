using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kolos_1.Controllers
{
    [Route("api/prescription")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        public readonly string conString = "Data Source=db-mssql;Initial Catalog=s19270;Integrated Security=True";
        [HttpGet]
        public IActionResult GetPrescriptions()
        {
            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select IdPrescription, Date, DueDate, Patient.LastName as PatientLastName, Doctor.LastName as DoctorLastName from Prescription " +
                                    "inner join Doctor on Doctor.IdDoctor = Prescription.IdDoctor " +
                                    "inner join Patient on Patient.IdPatient = Prescription.IdPatient " +
                                    "order by Date; ";
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                var list = new List<Prescription>();
                while (dr.Read())
                {
                    var pr = new Prescription();
                    pr.IdPrescription = dr["IdPrescription"].ToString();
                    pr.Date = DateTime.Parse(dr["Date"].ToString());
                    pr.DueDate = DateTime.Parse(dr["DueDate"].ToString());
                    pr.DoctorLastName = dr["DoctorLastName"].ToString();
                    pr.PatientLastName = dr["PatientLastName"].ToString();
                    list.Add(pr);
                }
                return Ok(list);
            }
        }
        [HttpGet("{doctorlastname}")]
        public IActionResult GetPrescription(string doctorlastname)
        {
            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =   "select IdPrescription, Date, DueDate, Patient.LastName as PatientLastName, Doctor.LastName as DoctorLastName from Prescription " +
                                    "inner join Doctor on Doctor.IdDoctor = Prescription.IdDoctor " +
                                    "inner join Patient on Patient.IdPatient = Prescription.IdPatient " +
                                    "where Doctor.LastName = @doctorlastname; ";
                com.Parameters.AddWithValue("doctorlastname", doctorlastname);
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                var list = new List<Prescription>();
                if (dr.Read())
                {
                    var pr = new Prescription();
                    pr.IdPrescription = dr["IdPrescription"].ToString();
                    pr.Date = DateTime.Parse(dr["Date"].ToString());
                    pr.DueDate = DateTime.Parse(dr["DueDate"].ToString());
                    pr.DoctorLastName = dr["DoctorLastName"].ToString();
                    pr.PatientLastName = dr["PatientLastName"].ToString();
                    return Ok(pr);
                }
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult NewPrescription(NewPrescription pr)
        {
            if (pr.Date == null || pr.DueDate == null ||
                pr.IdDoctor == null || pr.IdPatient == null ) return NotFound("Brak wszystkich danych");
            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand com = new SqlCommand())
            {
                con.Open();
                com.Connection = con;
                try
                {
                    com.CommandText = "exec NewPrescription @Date = @date, " +
                        "@DueDate = @duedate, " +
                        "@PatientId = @patientid, " +
                        "@DoctorId = @doctorid;";
                    com.Parameters.AddWithValue("Date", pr.Date);
                    com.Parameters.AddWithValue("DueDate", pr.DueDate);
                    com.Parameters.AddWithValue("PatientId", pr.IdPatient);
                    com.Parameters.AddWithValue("DoctorId", pr.IdDoctor);
                    com.ExecuteNonQuery();
                    return Ok("Dodano nowa recepte");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Wystapily bledy");
                }
                return NotFound("Wystapily bledy");
            }
        }

        /*create procedure NewPrescription @Date Date, @DueDate Date, @PatientId int, @DoctorId int
        as
        begin
        if @Date >= @DueDate or not exists(select 1 from Patient where IdPatient = @PatientId) or not exists(select 1 from Doctor where IdDoctor = @DoctorId)
        begin
        RAISERROR (15600,-1,-1, 'Brak poprawnych danych')
        return
        end
        declare @idp int = ((select max(IdPrescription) from Prescription) + 1)
        insert into Prescription (IdPrescription, Date, DueDate, IdPatient, IdDoctor)
        values (@idp, @Date, @DueDate, @PatientId, @DoctorId)
        end*/
    }
}