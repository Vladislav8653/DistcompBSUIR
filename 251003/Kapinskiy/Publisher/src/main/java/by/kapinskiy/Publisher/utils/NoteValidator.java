package by.kapinskiy.Publisher.utils;


import by.kapinskiy.Publisher.DTOs.Requests.IssueRequestDTO;
import by.kapinskiy.Publisher.DTOs.Requests.NoteRequestDTO;
import by.kapinskiy.Publisher.DTOs.Responses.NoteResponseDTO;
import by.kapinskiy.Publisher.services.IssuesService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.springframework.validation.Errors;
import org.springframework.validation.Validator;

@Component
public class NoteValidator implements Validator {
    private final IssuesService issuesService;

    @Autowired
    public NoteValidator(IssuesService issuesService) {
        this.issuesService = issuesService;
    }


    @Override
    public boolean supports(Class<?> clazz) {
        return NoteResponseDTO.class.equals(clazz);
    }

    @Override
    public void validate(Object target, Errors errors) {
        if (!errors.hasFieldErrors()){
            NoteRequestDTO note = (NoteRequestDTO) target;
            if (!issuesService.existsById(note.getIssueId())){
                errors.rejectValue("issueId", null, "Issue with such id doesn't exists");
            }
        }
    }
}
