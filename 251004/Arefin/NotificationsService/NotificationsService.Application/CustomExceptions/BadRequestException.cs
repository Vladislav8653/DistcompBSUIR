﻿namespace NotificationsService.Application.CustomExceptions;

public class BadRequestException(string message) : Exception(message);
