package com.example.rv1.exception;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ResponseStatus;

@ResponseStatus(HttpStatus.FORBIDDEN)
public class DublExeption extends RuntimeException{
    public DublExeption(String s){super(s);}
}
