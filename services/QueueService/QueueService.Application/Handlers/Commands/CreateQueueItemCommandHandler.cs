using AutoMapper;
using Azure.Core;
using Contracts.Doctors;
using Contracts.Patients;
using MediatR;
using QueueService.Application.Commands;
using QueueService.Application.Interfaces;
using QueueService.Domain.Entities;
using QueueService.Domain.Interfaces;
using QueueService.Domain.Interfaces.ExternalServices;
using QueueService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Commands
{
    public class CreateQueueItemCommandHandler : IRequestHandler<CreateQueueItemCommand, Guid>
    {
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDoctorServiceClient _doctorClient;
        private readonly IPatientServiceClient _patientClient;
        private readonly IAppointmentServiceClient _appointmentClient;
        private readonly IHospitalServiceClient _hospitalClient;
        private readonly IMapper _mapper;

        public CreateQueueItemCommandHandler(
            IQueueItemRepository queueItemRepository,
            IUnitOfWork unitOfWork,
            IDoctorServiceClient doctorClient,
            IPatientServiceClient patientClient,
            IHospitalServiceClient hospitalClient,
            IAppointmentServiceClient appointmentClient,
            IMapper mapper)
        {
            _queueItemRepository = queueItemRepository;
            _unitOfWork = unitOfWork;
            _doctorClient = doctorClient;
            _patientClient = patientClient;
            _appointmentClient = appointmentClient;
            _hospitalClient = hospitalClient;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateQueueItemCommand command, CancellationToken cancellationToken)
        {
            var patientInfoDto = await _patientClient.GetPatientInfoAsync(command.PatientId);
            var doctorInfoDto = await _doctorClient.GetDoctorInfoAsync(command.DoctorId);
            var appointmentInfo = await _appointmentClient.GetAppointmentInfoAsync(command.AppointmentId);
            var hospital = await _hospitalClient.GetHospitalByIdAsync(command.HospitalId);

            var patientInfo = _mapper.Map<PatientInfo>(patientInfoDto);
            var doctorInfo = _mapper.Map<DoctorInfo>(doctorInfoDto);

            var position = await _queueItemRepository.GetNextPositionAsync(command.DoctorId);
            var estimatedWait = TimeSpan.FromMinutes(position * 5);

            var queueItem = new QueueItem(
                Guid.NewGuid(),
                command.DoctorId,
                command.PatientId,
                command.AppointmentId,
                command.DepartmentId,
                command.HospitalId,
                position,
                estimatedWait,
                command.QueueNumber,
                patientInfo,
                doctorInfo
            );

            await _queueItemRepository.AddAsync(queueItem);
            await _unitOfWork.SaveAsync();

            return queueItem.Id;
        }
    }
}
