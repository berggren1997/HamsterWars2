﻿using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    public class HamsterService : IHamsterService
    {
        private readonly IRepositoryManager _repositoryManager;
        //private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;

        public HamsterService(IRepositoryManager repositoryManager, /*ILoggerManager loggerManager,*/
            IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            //_loggerManager = loggerManager;
            _mapper = mapper;
        }
        public async Task<IEnumerable<HamsterDto>> GetAllHamstersAsync(bool trackChanges)
        {
            var hamsters = await _repositoryManager.Hamster.GetAllHamstersAsync(trackChanges);

            var hamstersDto = _mapper.Map<IEnumerable<HamsterDto>>(hamsters);

            return hamstersDto;
        }
        public async Task<HamsterDto> GetHamsterByIdAsync(int id, bool trackChanges)
        {
            var hamster = await _repositoryManager.Hamster.GetHamsterByIdAsync(id, trackChanges);

            if (hamster == null)
                throw new HamsterNotFoundException(id);

            var hamsterDto = _mapper.Map<HamsterDto>(hamster);

            return hamsterDto;
        }

        public async Task<IEnumerable<HamsterDto>> GetTopFiveHamstersAsync(bool trackChanges)
        {
            var hamsters = await _repositoryManager.Hamster.GetTopFiveHamstersAsync(trackChanges);

            var hamstersDto = _mapper.Map<IEnumerable<HamsterDto>>(hamsters);

            return hamstersDto;
        }

        public async Task<IEnumerable<HamsterDto>> GetBottomFiveHamstersAsync(bool trackChanges)
        {
            var hamsters = await _repositoryManager.Hamster.GetBottomFiveHamstersAsync(trackChanges);

            var hamstersDto = _mapper.Map<IEnumerable<HamsterDto>>(hamsters);

            return hamstersDto;
        }

        public async Task<HamsterDto> CreateHamsterAsync(CreateHamsterDto newHamsterDto)
        {
            if(newHamsterDto == null)
                throw new CreateHamsterBadRequestException();
            
            var newHamsterEntity = _mapper.Map<Hamster>(newHamsterDto);
            
            _repositoryManager.Hamster.CreateHamster(newHamsterEntity);
            await _repositoryManager.SaveAsync();

            var newHamsterToReturn = _mapper.Map<HamsterDto>(newHamsterEntity);

            return newHamsterToReturn;
        }
        public async Task DeleteHamsterAsync(int id, bool trackChanges) 
        {
            var hamster = await _repositoryManager.Hamster.GetHamsterByIdAsync(id, trackChanges);

            if (hamster == null)
                throw new HamsterNotFoundException(id);

            _repositoryManager.Hamster.DeleteHamster(hamster);

            await _repositoryManager.SaveAsync();
        }

        public async Task UpdateHamsterAsync(int id, UpdateHamsterDto updateHamsterDto, bool trackChanges)
        {
            var hamster = await _repositoryManager.Hamster.GetHamsterByIdAsync(id, trackChanges);

            if (hamster == null)
                throw new HamsterNotFoundException(id);

            _mapper.Map(updateHamsterDto, hamster);
            await _repositoryManager.SaveAsync();
        }
    }
}