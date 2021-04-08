﻿using AutoMapper;
using Gym.Core.Entities;
using Gym.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Data.Data
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //CreateMap<GymClass, GymClassesViewModel>()
            //    .ForMember(dest => dest.Attending, opt => opt.Ignore()); 

            //CreateMap<GymClass, GymClassesViewModel>()
            //    .ForMember(dest => dest.Attending,
            //     opt => opt.MapFrom(
            //         (src, dest, _, context) => src.AttendingMembers.Any
            //         (a => a.ApplicationUserId == context.Items["Id"].ToString())));            

            CreateMap<GymClass, GymClassesViewModel>()
                .ForMember(dest => dest.Attending,
                 opt => opt.MapFrom<AttendingResolver>());

            CreateMap<IEnumerable<GymClass>, IndexViewModel>()
                .ForMember(
                    dest => dest.GymClasses,
                    from => from.MapFrom(g => g.ToList()))
                 .ForMember(
                    dest => dest.ShowHistory, opt => opt.Ignore());
        }
    }

    public class AttendingResolver : IValueResolver<GymClass, GymClassesViewModel, bool>
    {
        public bool Resolve(GymClass source, GymClassesViewModel destination, bool destMember, ResolutionContext context)
        {
            if (source.AttendingMembers == null || context.Items.Count == 0) return false;
            return source.AttendingMembers.Any(a => a.ApplicationUserId == context.Items["Id"].ToString());
        }
    }
}
