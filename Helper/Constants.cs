using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Helper
{
    public static class Constants
    {
        //Shoulder
        public const int Sh_Flex_Current = 15;
        public const int Sh_Flex_Goal = 165;

        public const int Sh_ExRot_Current = 0;
        public const int Sh_ExRot_Goal = 220;

        public const int Sh_Abd_Current = 0;
        public const int Sh_Abd_Goal = 160;

        //Knee
        public const int Knee_Flex_Current = 90;
        public const int Knee_Flex_Goal = 132;

        public const int Knee_Ext_Current = 90;
        public const int Knee_Ext_Goal = -10;


        public const int Knee_Flex_Current_Start = 10;
        public const int Knee_Flex_Current_End = 120;
        public const int Knee_Flex_Goal_Start = 20;
        public const int Knee_Flex_Goal_End = 140;

        public const int Knee_Ext_Current_Start = 110;
        public const int Knee_Ext_Current_End = 0;
        public const int Knee_Ext_Goal_Start = 110;
        public const int Knee_Ext_Goal_End = -10;

        //Ankle
        public const int Ankle_Flex_Current = 5;
        public const int Ankle_Flex_Goal = 50;

        public const int Ankle_Ext_Current = 5;
        public const int Ankle_Ext_Goal = -20;


        public const int Ankle_Flex_Current_Start = 5;
        public const int Ankle_Flex_Current_End = 50;
        public const int Ankle_Flex_Goal_Start = 5;
        public const int Ankle_Flex_Goal_End = 50;

        public const int Ankle_Ext_Current_Start = 5;
        public const int Ankle_Ext_Current_End = -20;
        public const int Ankle_Ext_Goal_Start = 5;
        public const int Ankle_Ext_Goal_End = -20;

    }

    public static class ConstantsVar
    {
        public const int Admin = 0;
        public const int Support = 1;
        public const int Therapist = 2;
        public const int Provider = 3;
        public const int Installer = 4;
        public const int Patient = 5;
        public const int PatientAdministrator = 6;

    }
    public static class PlaneProperty
    {
        public const int SagittalAngle = 1;
        public const int ScapularAngle = 2;
        public const int FrontalAngle = 3;

    }
    public static class AppointmentConstants
    {
        public const int Requested = 0;
        public const int SlotsReceived = 1;
        public const int SlotAccepted = 2;
        public const int Completed = 3;
        public const int RescheduleRequested = 4;
        public const int Expired = 5;
        public const int Cancelled = 6;
    }
    public static class AppointmentTypeConstants
    {
        public const int Therapist = 0;
        public const int Support = 1;
    }

}
