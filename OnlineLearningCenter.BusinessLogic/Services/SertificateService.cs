using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _certificateRepository;
    private readonly IMapper _mapper;

    public CertificateService(ICertificateRepository certificateRepository, IMapper mapper)
    {
        _certificateRepository = certificateRepository;
        _mapper = mapper;
    }

    public async Task<CertificateDto> CreateCertificateAsync(CreateCertificateDto certificateDto)
    {
        var certificate = _mapper.Map<Certificate>(certificateDto);
        await _certificateRepository.AddAsync(certificate);

        var newCertificate = await _certificateRepository.GetCertificateByIdWithDetailsAsync(certificate.CertificateId);
        return _mapper.Map<CertificateDto>(newCertificate);
    }

    public async Task DeleteCertificateAsync(int certificateId)
    {
        await _certificateRepository.DeleteAsync(certificateId);
    }

    public async Task<CertificateDto?> GetCertificateByIdAsync(int certificateId)
    {
        var certificate = await _certificateRepository.GetCertificateByIdWithDetailsAsync(certificateId);
        return _mapper.Map<CertificateDto>(certificate);
    }

    public async Task<IEnumerable<CertificateDto>> GetCertificatesByStudentIdAsync(int studentId)
    {
        var certificates = await _certificateRepository.GetCertificatesByStudentIdWithDetailsAsync(studentId);
        return _mapper.Map<IEnumerable<CertificateDto>>(certificates);
    }

    public async Task UpdateCertificateAsync(UpdateCertificateDto certificateDto)
    {
        var existingCertificate = await _certificateRepository.GetByIdAsync(certificateDto.CertificateId);
        if (existingCertificate == null)
        {
            throw new KeyNotFoundException("Сертификат не найден.");
        }

        existingCertificate.CertificateUrl = certificateDto.CertificateUrl;

        await _certificateRepository.UpdateAsync(existingCertificate);
    }
}