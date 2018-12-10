using OneDirect.Helper;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository
{
    public class EquipmentExerciseRepository : IEquipmentExerciseInterface
    {
        private readonly OneDirectContext context1;

        public EquipmentExerciseRepository(OneDirectContext context1)
        {
            this.context1 = context1;
        }
        public List<EquipmentExercise> GetEquipmentExerciseString()
        {
            return (from p in context1.EquipmentExercise
                    select p).ToList();
        }
        public List<ExcerciseProtocol> GetExcerciseProtocol()
        {

            List<ExcerciseProtocol> lresult = new List<ExcerciseProtocol>();
            List<EquipmentExercise> lexerciseString = (from p in context1.EquipmentExercise
                                                       select p).OrderByDescending(x => x.ExerciseEnum).ToList();
            if (lexerciseString != null && lexerciseString.Count > 0)
            {
                foreach (EquipmentExercise exercise in lexerciseString)
                {
                    if (exercise.Limb == "Knee" || exercise.Limb == "Ankle")
                    {
                        ExcerciseProtocol lexercise = new ExcerciseProtocol();
                        lexercise.Limb = exercise.Limb;
                        lexercise.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise.ProtocolName = exercise.FlexionString;
                        lexercise.ProtocolEnum = 1;
                        lexercise.LaymanName = exercise.FlexionLaymanString;
                        lresult.Add(lexercise);

                        ExcerciseProtocol lexercise1 = new ExcerciseProtocol();
                        lexercise1.Limb = exercise.Limb;
                        lexercise1.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise1.ProtocolName = exercise.ExtensionString;
                        lexercise1.ProtocolEnum = 2;
                        lexercise1.LaymanName = exercise.ExtensionLaymanString;
                        lresult.Add(lexercise1);

                        ExcerciseProtocol lexercise2 = new ExcerciseProtocol();
                        lexercise2.Limb = exercise.Limb;
                        lexercise2.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise2.ProtocolName = exercise.FlexionString + "-" + exercise.ExtensionString;
                        lexercise2.ProtocolEnum = 3;
                        lexercise2.LaymanName = exercise.FlexionLaymanString + "-" + exercise.ExtensionLaymanString;
                        lresult.Add(lexercise2);
                    }
                    else
                    {
                        ExcerciseProtocol lexercise = new ExcerciseProtocol();
                        lexercise.Limb = exercise.Limb;
                        lexercise.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise.ProtocolName = exercise.FlexionString;
                        lexercise.ProtocolEnum = 1;
                        lexercise.LaymanName = exercise.FlexionLaymanString;
                        lresult.Add(lexercise);
                    }
                }
            }
            return lresult;
        }

        public List<EquipmentExcercise> GetEquipmentExcercise()
        {
            List<EquipmentExcercise> lresult = new List<EquipmentExcercise>();
            List<EquipmentExercise> lexerciseString = (from p in context1.EquipmentExercise
                                                       select p).OrderByDescending(x=>x.ExerciseEnum).ToList();
            if (lexerciseString != null && lexerciseString.Count > 0)
            {
                foreach (EquipmentExercise exercise in lexerciseString)
                {
                    if (exercise.Limb == "Knee" || exercise.Limb == "Ankle")
                    {
                        EquipmentExcercise lexercise = new EquipmentExcercise();
                        lexercise.Limb = exercise.Limb;
                        lexercise.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise.ExcerciseName = exercise.FlexionString + "-" + exercise.ExtensionString;
                        lexercise.EquipmentName = "";
                        lresult.Add(lexercise);
                    }
                    else
                    {
                        EquipmentExcercise lexercise = new EquipmentExcercise();
                        lexercise.Limb = exercise.Limb;
                        lexercise.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise.ExcerciseName = exercise.FlexionString;
                        lexercise.EquipmentName = "";
                        lresult.Add(lexercise);
                    }
                }
            }
            return lresult;
        }


        public List<ExcerciseProtocol> GetExcerciseProtocol(List<EquipmentExercise> lexerciseString)
        {

            List<ExcerciseProtocol> lresult = new List<ExcerciseProtocol>();
            if (lexerciseString != null && lexerciseString.Count > 0)
            {
                foreach (EquipmentExercise exercise in lexerciseString)
                {
                    if (exercise.Limb == "Knee" || exercise.Limb == "Ankle")
                    {
                        ExcerciseProtocol lexercise = new ExcerciseProtocol();
                        lexercise.Limb = exercise.Limb;
                        lexercise.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise.ProtocolName = exercise.FlexionString;
                        lexercise.ProtocolEnum = 1;
                        lexercise.LaymanName = exercise.FlexionLaymanString;
                        lresult.Add(lexercise);

                        ExcerciseProtocol lexercise1 = new ExcerciseProtocol();
                        lexercise1.Limb = exercise.Limb;
                        lexercise1.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise1.ProtocolName = exercise.ExtensionString;
                        lexercise1.ProtocolEnum = 2;
                        lexercise1.LaymanName = exercise.ExtensionLaymanString;
                        lresult.Add(lexercise1);

                        ExcerciseProtocol lexercise2 = new ExcerciseProtocol();
                        lexercise2.Limb = exercise.Limb;
                        lexercise2.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise2.ProtocolName = exercise.FlexionString + "-" + exercise.ExtensionString;
                        lexercise2.ProtocolEnum = 3;
                        lexercise2.LaymanName = exercise.FlexionLaymanString + "-" + exercise.ExtensionLaymanString;
                        lresult.Add(lexercise2);
                    }
                    else
                    {
                        ExcerciseProtocol lexercise = new ExcerciseProtocol();
                        lexercise.Limb = exercise.Limb;
                        lexercise.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise.ProtocolName = exercise.FlexionString;
                        lexercise.ProtocolEnum = 1;
                        lexercise.LaymanName = exercise.FlexionLaymanString;
                        lresult.Add(lexercise);
                    }
                }
            }
            return lresult;
        }

        public List<EquipmentExcercise> GetEquipmentExcercise(List<EquipmentExercise> lexerciseString)
        {
            List<EquipmentExcercise> lresult = new List<EquipmentExcercise>();

            if (lexerciseString != null && lexerciseString.Count > 0)
            {
                foreach (EquipmentExercise exercise in lexerciseString)
                {
                    if (exercise.Limb == "Knee" || exercise.Limb == "Ankle")
                    {
                        EquipmentExcercise lexercise = new EquipmentExcercise();
                        lexercise.Limb = exercise.Limb;
                        lexercise.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise.ExcerciseName = exercise.FlexionString + "-" + exercise.ExtensionString;
                        lexercise.EquipmentName = "";
                        lresult.Add(lexercise);
                    }
                    else
                    {
                        EquipmentExcercise lexercise = new EquipmentExcercise();
                        lexercise.Limb = exercise.Limb;
                        lexercise.ExcerciseEnum = exercise.ExerciseEnum;
                        lexercise.ExcerciseName = exercise.FlexionString;
                        lexercise.EquipmentName = "";
                        lresult.Add(lexercise);
                    }
                }
            }
            return lresult;
        }
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context1.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
