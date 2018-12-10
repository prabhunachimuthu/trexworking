using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Extensions
{
    public static class UserExtension
    {
        public static UserViewModel UserToUserViewModel(this User user)
        {
            if (user == null)
                return null;

            UserViewModel uv = new UserViewModel()
            {
                UserId = user.UserId,
                Type = user.Type,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Password = user.Password,
                Npi = user.Npi,
                Vseeid = user.Vseeid

            };
            return uv;
        }

        public static List<UserViewModel> UserToUserViewModelList(this List<User> puser)
        {
            List<UserViewModel> uv = new List<UserViewModel>();
            if (puser == null)
                return null;
            foreach (User user in puser)
            {
                UserViewModel uvm = new UserViewModel();
                uvm.UserId = user.UserId;
                uvm.Type = user.Type;
                uvm.Name = user.Name;
                uvm.Email = user.Email;
                uvm.Phone = user.Phone;
                uvm.Address = user.Address;
                uvm.Password = user.Password;
                uvm.Npi = user.Npi;
                uvm.Vseeid = user.Vseeid;
                uvm.Therapist = null;
                uv.Add(uvm);
            }
            return uv;
        }

        public static User UserViewModelToUser(this UserViewModel user)
        {
            if (user == null)
                return null;

            User uv = new User()
            {
                UserId = user.UserId,
                Type = user.Type,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Password = user.Password,
                Npi = user.Npi,
                Vseeid = user.Vseeid
            };
            return uv;
        }

      

        public static EquipmentAssignment EquipmentViewToEquipment(this EquipmentView _equipment)
        {
            if (_equipment == null)
                return null;

            EquipmentAssignment ev = new EquipmentAssignment()
            {
                AssignmentId = _equipment.AssignmentId
               
            };
            return ev;
        }

        public static EquipmentView EquipmentToEquipmentAssignmentExtension(this EquipmentAssignment _equipment)
        {
            if (_equipment == null)
                return null;

            EquipmentView ev = new EquipmentView()
            {
                AssignmentId = _equipment.AssignmentId
                
            };
            return ev;
        }

        public static Protocol ProtocolViewToProtocol(this ProtocolView _protocol)
        {
            if (_protocol == null)
                return null;

            Protocol pv = new Protocol()
            {

                ProtocolId = _protocol.ProtocolId,
                ProtocolName = _protocol.ProtocolName,
                RestPosition = _protocol.RestPosition,
                MaxUpLimit = _protocol.MaxUpLimit,
                StretchUpLimit = _protocol.StretchUpLimit,
                MaxDownLimit = _protocol.MaxDownLimit,
                StretchDownLimit = _protocol.StretchDownLimit,
                RestAt = _protocol.RestAt,
                RepsAt = _protocol.RepsAt,
                Speed = _protocol.Speed,
                Reps = _protocol.Reps,
                
                FlexUpLimit = _protocol.FlexUpLimit,
                
                StretchUpHoldtime = _protocol.StretchUpHoldtime,
                FlexDownLimit = _protocol.FlexDownLimit,
                
                StretchDownHoldtime = _protocol.StretchDownHoldtime,
                
            };
            return pv;
        }
        public static ProtocolView ProtocolToProtocolView(this Protocol _protocol)
        {
            if (_protocol == null)
                return null;

            ProtocolView pv = new ProtocolView()
            {
                ProtocolId = _protocol.ProtocolId,
                ProtocolName = _protocol.ProtocolName,
                RestPosition = _protocol.RestPosition,
                MaxUpLimit = _protocol.MaxUpLimit,
                StretchUpLimit = _protocol.StretchUpLimit,
                MaxDownLimit = _protocol.MaxDownLimit,
                StretchDownLimit = _protocol.StretchDownLimit,
                RestAt = _protocol.RestAt,
                RepsAt = _protocol.RepsAt,
                Speed = _protocol.Speed,
                Reps = _protocol.Reps,
              
                FlexUpLimit = _protocol.FlexUpLimit,
                
                StretchUpHoldtime = _protocol.StretchUpHoldtime,
                FlexDownLimit = _protocol.FlexDownLimit,
                
                StretchDownHoldtime = _protocol.StretchDownHoldtime,
                
            };
            return pv;
        }
    }
}
