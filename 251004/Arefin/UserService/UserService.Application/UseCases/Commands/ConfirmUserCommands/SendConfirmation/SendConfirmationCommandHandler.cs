﻿using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Exceptions;
using UserService.Application.Producers.EmailProducer;
using UserService.Domain.Models;

namespace UserService.Application.UseCases.Commands.ConfirmUserCommands.SendConfirmation;

public class SendConfirmationCommandHandler(
    KafkaEmailProducer kafkaEmailProducer,
    UserManager<User> userManager) : 
    IRequestHandler<SendConfirmationCommand, string>
{
    public async Task<string> Handle(SendConfirmationCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.UserId, out var userIdGuid))
        {
            throw new ValidationException("UserId is invalid");
        }

        var user = await userManager.FindByIdAsync(userIdGuid.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User with id {request.UserId} not found");
        }

        if (user.EmailConfirmed)
        {
            throw new NotFoundException($"User with email {user.Email} already confirmed");
        }
        
        var emailBody = $"Your confirmation code: {await userManager.GenerateEmailConfirmationTokenAsync(user)}";

        await smtpService.SendEmailAsync(user.UserName!, user.Email!, "Confirm your email", emailBody);
        return "Your confirmation code was sent on email.";
    }
}