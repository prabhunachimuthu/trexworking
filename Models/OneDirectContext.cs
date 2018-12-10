using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneDirect.Models
{
    public partial class OneDirectContext : DbContext
    {
        public virtual DbSet<ActivityLog> ActivityLog { get; set; }
        public virtual DbSet<AppointmentSchedule> AppointmentSchedule { get; set; }
        public virtual DbSet<Availability> Availability { get; set; }
        public virtual DbSet<DeviceCalibration> DeviceCalibration { get; set; }
        public virtual DbSet<EquipmentAssignment> EquipmentAssignment { get; set; }
        public virtual DbSet<EquipmentExercise> EquipmentExercise { get; set; }
        public virtual DbSet<Library> Library { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<Pain> Pain { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<PatientConfiguration> PatientConfiguration { get; set; }
        public virtual DbSet<PatientLibrary> PatientLibrary { get; set; }
        public virtual DbSet<PatientReview> PatientReview { get; set; }
        public virtual DbSet<PatientRx> PatientRx { get; set; }
        public virtual DbSet<Protocol> Protocol { get; set; }
        public virtual DbSet<RomchangeLog> RomchangeLog { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<SessionAuditTrail> SessionAuditTrail { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserActivityLog> UserActivityLog { get; set; }
        public virtual DbSet<Vtransact> Vtransact { get; set; }

        public OneDirectContext(DbContextOptions<OneDirectContext> options)
    : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseNpgsql(@"Host=ec2-54-83-8-246.compute-1.amazonaws.com;Port=5432;Database=ddh2qscke6khal;User ID=jmbezzzwpaicso;Password=3c70e292d3c61592c6f5512bfe9bfe3cc66fdde9d05d4b618492b4b5df5ac896;sslmode=Require;Trust Server Certificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(e => e.TransactionId);

                entity.HasIndex(e => e.PatientId)
                    .HasName("fki_fk_User_Patient");

                entity.HasIndex(e => e.UserId)
                    .HasName("fki_fk_User_ActivityLog");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("TransactionID")
                    .ValueGeneratedNever();

                entity.Property(e => e.LinkToActivity)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.ActivityLog)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("fk_User_Patient");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ActivityLog)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_ActivityLog");
            });

            modelBuilder.Entity<AppointmentSchedule>(entity =>
            {
                entity.HasKey(e => e.AppointmentId);

                entity.HasIndex(e => e.PatientId)
                    .HasName("fki_fk_Patient_AppointmentSchedule");

                entity.HasIndex(e => e.UserId)
                    .HasName("fki_fk_User_AppointmentSchedule");

                entity.Property(e => e.AppointmentId)
                    .HasColumnName("AppointmentID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CallStatus).IsRequired();

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.RecordedFile).IsRequired();

                entity.Property(e => e.SlotStatus).IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.Property(e => e.UserType).IsRequired();

                entity.Property(e => e.VseeUrl).HasColumnName("VseeURL");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.AppointmentSchedule)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("fk_Patient_AppointmentSchedule");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AppointmentSchedule)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_AppointmentSchedule");
            });

            modelBuilder.Entity<Availability>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("fki_fk_User_Availability");

                entity.Property(e => e.AvailabilityId)
                    .HasColumnName("AvailabilityID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DayOfWeek).IsRequired();

                entity.Property(e => e.HourOfDay).IsRequired();

                entity.Property(e => e.TimeZoneOffset).IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.Property(e => e.UserType).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Availability)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_Availability");
            });

            modelBuilder.Entity<DeviceCalibration>(entity =>
            {
                entity.HasKey(e => e.SetupId);

                entity.Property(e => e.SetupId)
                    .HasColumnName("SetupID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Actuator1ExtendedAngle).HasColumnName("Actuator1_Extended_Angle");

                entity.Property(e => e.Actuator1ExtendedPulse).HasColumnName("Actuator1_Extended_Pulse");

                entity.Property(e => e.Actuator1NeutralAngle).HasColumnName("Actuator1_Neutral_Angle");

                entity.Property(e => e.Actuator1NeutralPulse).HasColumnName("Actuator1_Neutral_Pulse");

                entity.Property(e => e.Actuator1RetractedAngle).HasColumnName("Actuator1_Retracted_Angle");

                entity.Property(e => e.Actuator1RetractedPulse).HasColumnName("Actuator1_Retracted_Pulse");

                entity.Property(e => e.Actuator2ExtendedAngle).HasColumnName("Actuator2_Extended_Angle");

                entity.Property(e => e.Actuator2ExtendedPulse).HasColumnName("Actuator2_Extended_Pulse");

                entity.Property(e => e.Actuator2NeutralAngle).HasColumnName("Actuator2_Neutral_Angle");

                entity.Property(e => e.Actuator2NeutralPulse).HasColumnName("Actuator2_Neutral_Pulse");

                entity.Property(e => e.Actuator2RetractedAngle).HasColumnName("Actuator2_Retracted_Angle");

                entity.Property(e => e.Actuator2RetractedPulse).HasColumnName("Actuator2_Retracted_Pulse");

                entity.Property(e => e.Actuator3ExtendedAngle).HasColumnName("Actuator3_Extended_Angle");

                entity.Property(e => e.Actuator3ExtendedPulse).HasColumnName("Actuator3_Extended_Pulse");

                entity.Property(e => e.Actuator3NeutralAngle).HasColumnName("Actuator3_Neutral_Angle");

                entity.Property(e => e.Actuator3NeutralPulse).HasColumnName("Actuator3_Neutral_Pulse");

                entity.Property(e => e.Actuator3RetractedAngle).HasColumnName("Actuator3_Retracted_Angle");

                entity.Property(e => e.Actuator3RetractedPulse).HasColumnName("Actuator3_Retracted_Pulse");

                entity.Property(e => e.BoomId1)
                    .IsRequired()
                    .HasColumnName("BoomID1");

                entity.Property(e => e.BoomId2).HasColumnName("BoomID2");

                entity.Property(e => e.BoomId3).HasColumnName("BoomID3");

                entity.Property(e => e.ChairId)
                    .IsRequired()
                    .HasColumnName("ChairID");

                entity.Property(e => e.ControllerId)
                    .IsRequired()
                    .HasColumnName("ControllerID");

                entity.Property(e => e.DeviceConfiguration).IsRequired();

                entity.Property(e => e.EquipmentType).IsRequired();

                entity.Property(e => e.InstallerId)
                    .IsRequired()
                    .HasColumnName("InstallerID");

                entity.Property(e => e.MacAddress).IsRequired();

                entity.Property(e => e.NewControllerId).HasColumnName("NewControllerID");

                entity.Property(e => e.PatientSide).IsRequired();

                entity.Property(e => e.TabletId)
                    .IsRequired()
                    .HasColumnName("TabletID")
                    .HasDefaultValueSql("''::text");
            });

            modelBuilder.Entity<EquipmentAssignment>(entity =>
            {
                entity.HasKey(e => e.AssignmentId);

                entity.Property(e => e.AssignmentId)
                    .HasColumnName("AssignmentID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Boom1Id).HasColumnName("Boom1ID");

                entity.Property(e => e.Boom2Id).HasColumnName("Boom2ID");

                entity.Property(e => e.Boom3Id).HasColumnName("Boom3ID");

                entity.Property(e => e.ChairId).HasColumnName("ChairID");

                entity.Property(e => e.DateInstalled).HasColumnType("date");

                entity.Property(e => e.DateRemoved).HasColumnType("date");

                entity.Property(e => e.InstallerId)
                    .IsRequired()
                    .HasColumnName("InstallerID");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.HasOne(d => d.Installer)
                    .WithMany(p => p.EquipmentAssignment)
                    .HasForeignKey(d => d.InstallerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fx_therapist_user");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.EquipmentAssignment)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fx_patient_user");
            });

            modelBuilder.Entity<EquipmentExercise>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ExerciseEnum).IsRequired();

                entity.Property(e => e.ExtensionLaymanString).IsRequired();

                entity.Property(e => e.ExtensionString).IsRequired();

                entity.Property(e => e.FlexionLaymanString).IsRequired();

                entity.Property(e => e.FlexionString).IsRequired();

                entity.Property(e => e.Limb).IsRequired();
            });

            modelBuilder.Entity<Library>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Exercise).IsRequired();

                entity.Property(e => e.Limb).IsRequired();

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("URL");
            });

            modelBuilder.Entity<Messages>(entity =>
            {
                entity.HasKey(e => e.MsgHeaderId);

                entity.Property(e => e.MsgHeaderId).HasColumnName("MsgHeaderID");

                entity.Property(e => e.BodyText).IsRequired();

                entity.Property(e => e.PatientId)
                    .IsRequired()
                    .HasColumnName("PatientID");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.Property(e => e.UserName).IsRequired();

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.MessagesPatient)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fx_patient_user");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MessagesUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fx_user_userId");
            });

            modelBuilder.Entity<Pain>(entity =>
            {
                entity.Property(e => e.PainId).ValueGeneratedNever();

                entity.Property(e => e.ProtocolId)
                    .IsRequired()
                    .HasColumnName("ProtocolID");

                entity.Property(e => e.RxId).HasColumnName("RxID");

                entity.Property(e => e.SessionId)
                    .IsRequired()
                    .HasColumnName("SessionID");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Pain)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Session_Pain");
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.EncryptPin).HasColumnName("EncryptPIN");

                entity.Property(e => e.Paid).HasColumnName("PAID");

                entity.Property(e => e.PatientLoginId)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.PatientName).IsRequired();

                entity.Property(e => e.Pin).HasColumnName("PIN");

                entity.Property(e => e.ProviderId)
                    .IsRequired()
                    .HasColumnName("ProviderID");

                entity.Property(e => e.Ssn).HasColumnName("SSN");

                entity.Property(e => e.Therapistid).HasColumnName("therapistid");
            });

            modelBuilder.Entity<PatientConfiguration>(entity =>
            {
                entity.HasIndex(e => e.InstallerId)
                    .HasName("fki_fx_User_PatientConfiguration");

                entity.HasIndex(e => e.PatientId)
                    .HasName("fki_fk_Patient_PatientConfiguration");

                entity.HasIndex(e => e.SetupId)
                    .HasName("fki_fk_DeviceCalibration_PatientConfiguration");

                entity.Property(e => e.DeviceConfiguration).IsRequired();

                entity.Property(e => e.EquipmentType).IsRequired();

                entity.Property(e => e.InstallerId)
                    .IsRequired()
                    .HasColumnName("InstallerID")
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.PatientFirstName)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.PatientSide).IsRequired();

                entity.Property(e => e.RxId)
                    .IsRequired()
                    .HasColumnName("RxID");

                entity.Property(e => e.SetupId)
                    .IsRequired()
                    .HasColumnName("SetupID");

                entity.Property(e => e.UserMode).HasDefaultValueSql("0");

                entity.HasOne(d => d.Installer)
                    .WithMany(p => p.PatientConfiguration)
                    .HasForeignKey(d => d.InstallerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fx_User_PatientConfiguration");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientConfiguration)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Patient_PatientConfiguration");

                entity.HasOne(d => d.Setup)
                    .WithMany(p => p.PatientConfiguration)
                    .HasForeignKey(d => d.SetupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_DeviceCalibration_PatientConfiguration");
            });

            modelBuilder.Entity<PatientLibrary>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Exercise).IsRequired();

                entity.Property(e => e.Limb).IsRequired();

                entity.Property(e => e.Patient).IsRequired();

                entity.Property(e => e.Side).IsRequired();

                entity.Property(e => e.Status).HasDefaultValueSql("0");

                entity.Property(e => e.Type).IsRequired();

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("URL");

                entity.Property(e => e.UserId).IsRequired();
            });

            modelBuilder.Entity<PatientReview>(entity =>
            {
                entity.HasKey(e => e.ReviewId);

                entity.Property(e => e.ReviewId)
                    .HasColumnName("ReviewID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActivityType).IsRequired();

                entity.Property(e => e.PatientId)
                    .IsRequired()
                    .HasColumnName("PatientID");

                entity.Property(e => e.PatientName).IsRequired();

                entity.Property(e => e.SessionId)
                    .IsRequired()
                    .HasColumnName("SessionID");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.Property(e => e.UserName).IsRequired();

                entity.Property(e => e.UserType).IsRequired();
            });

            modelBuilder.Entity<PatientRx>(entity =>
            {
                entity.HasKey(e => e.RxId);

                entity.Property(e => e.RxId)
                    .HasColumnName("RxID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CurrentExtension).HasDefaultValueSql("0");

                entity.Property(e => e.CurrentFlexion).HasDefaultValueSql("0");

                entity.Property(e => e.DeviceConfiguration)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.GoalExtension).HasDefaultValueSql("0");

                entity.Property(e => e.GoalFlexion).HasDefaultValueSql("0");

                entity.Property(e => e.PainThreshold).HasDefaultValueSql("0");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

                entity.Property(e => e.RateOfChange).HasDefaultValueSql("0");

                entity.Property(e => e.RxDaysPerweek).HasDefaultValueSql("0");

                entity.Property(e => e.RxSessionsPerWeek).HasDefaultValueSql("0");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientRx)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Patient_Patientrx");

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.PatientRx)
                    .HasForeignKey(d => d.ProviderId)
                    .HasConstraintName("fk_User_PatientRx");
            });

            modelBuilder.Entity<Protocol>(entity =>
            {
                entity.HasIndex(e => e.PatientId)
                    .HasName("fki_Patient_Protocol");

                entity.Property(e => e.ProtocolId)
                    .HasColumnName("ProtocolID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DeviceConfiguration)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.DownReps).HasDefaultValueSql("0");

                entity.Property(e => e.EquipmentType)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.FlexUpHoldtime).HasDefaultValueSql("0");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.Plane).HasDefaultValueSql("0");

                entity.Property(e => e.ProtocolEnum).HasDefaultValueSql("0");

                entity.Property(e => e.ProtocolName).IsRequired();

                entity.Property(e => e.Reps).HasDefaultValueSql("0");

                entity.Property(e => e.RepsAt).HasDefaultValueSql("0");

                entity.Property(e => e.RestAt).HasDefaultValueSql("0");

                entity.Property(e => e.RestPosition).HasDefaultValueSql("0");

                entity.Property(e => e.RestTime).HasDefaultValueSql("0");

                entity.Property(e => e.RxId)
                    .IsRequired()
                    .HasColumnName("RxID");

                entity.Property(e => e.Speed).HasDefaultValueSql("0");

                entity.Property(e => e.StretchUpHoldtime).HasDefaultValueSql("0");

                entity.Property(e => e.UpReps).HasDefaultValueSql("0");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Protocol)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Patient_Protocol");

                entity.HasOne(d => d.Rx)
                    .WithMany(p => p.Protocol)
                    .HasForeignKey(d => d.RxId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Patientrx_Protocol");
            });

            modelBuilder.Entity<RomchangeLog>(entity =>
            {
                entity.ToTable("ROMChangeLog");

                entity.Property(e => e.ChangedBy).IsRequired();

                entity.Property(e => e.RxId)
                    .IsRequired()
                    .HasColumnName("RxID");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasIndex(e => e.PatientId)
                    .HasName("fki_Patient_Session");

                entity.Property(e => e.SessionId)
                    .HasColumnName("SessionID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ProtocolId)
                    .IsRequired()
                    .HasColumnName("ProtocolID");

                entity.Property(e => e.RxId)
                    .IsRequired()
                    .HasColumnName("RxID");

                entity.Property(e => e.TimeZoneOffset)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Patient_Session");

                entity.HasOne(d => d.Protocol)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.ProtocolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Protocol_Session");

                entity.HasOne(d => d.Rx)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.RxId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PatientRx_Session");
            });

            modelBuilder.Entity<SessionAuditTrail>(entity =>
            {
                entity.HasKey(e => e.AuditTrailId);

                entity.Property(e => e.AuditTrailId).HasColumnName("AuditTrailID");

                entity.Property(e => e.EmailId)
                    .IsRequired()
                    .HasColumnName("EmailID");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.PasswordUsed).IsRequired();

                entity.Property(e => e.SessionId)
                    .IsRequired()
                    .HasColumnName("SessionID");

                entity.Property(e => e.SessionStatus).IsRequired();

                entity.Property(e => e.SessionType).IsRequired();

                entity.Property(e => e.Type).IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Npi).HasColumnName("NPI");

                entity.Property(e => e.Vseeid).HasColumnName("vseeid");
            });

            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.HasKey(e => e.ActivityId);

                entity.HasIndex(e => e.ReviewId)
                    .HasName("fki_fx_UserActivityLog_PatientReview");

                entity.Property(e => e.ActivityId).HasColumnName("ActivityID");

                entity.Property(e => e.ActivityType).IsRequired();

                entity.Property(e => e.RecordExistingJson).HasColumnName("RecordExistingJSON");

                entity.Property(e => e.RecordJson).HasColumnName("RecordJSON");

                entity.Property(e => e.ReviewId).HasColumnName("ReviewID");

                entity.Property(e => e.SessionId)
                    .IsRequired()
                    .HasColumnName("SessionID");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.Property(e => e.UserName).IsRequired();

                entity.Property(e => e.UserType).IsRequired();

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.UserActivityLog)
                    .HasForeignKey(d => d.ReviewId)
                    .HasConstraintName("fx_UserActivityLog_PatientReview");
            });

            modelBuilder.Entity<Vtransact>(entity =>
            {
                entity.ToTable("VTransact");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.SessionId)
                    .IsRequired()
                    .HasColumnName("SessionID");

                entity.Property(e => e.TherapistId)
                    .IsRequired()
                    .HasColumnName("TherapistID");

                entity.Property(e => e.Token).IsRequired();
            });
        }
    }
}
