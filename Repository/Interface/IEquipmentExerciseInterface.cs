using OneDirect.Helper;
using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    public interface IEquipmentExerciseInterface : IDisposable
    {
        List<EquipmentExercise> GetEquipmentExerciseString();
        List<ExcerciseProtocol> GetExcerciseProtocol();
        List<EquipmentExcercise> GetEquipmentExcercise();
        List<ExcerciseProtocol> GetExcerciseProtocol(List<EquipmentExercise> lexerciseString);
        List<EquipmentExcercise> GetEquipmentExcercise(List<EquipmentExercise> lexerciseString);
    }
}
