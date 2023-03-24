import { FormControl, FormLabel } from "@chakra-ui/react";
import { SubjectDropDown } from "components/subject";
import { Dispatch, RefObject, SetStateAction } from "react";

function MsgCopyForm(props: {
  subjectInputRef: RefObject<HTMLSelectElement>;
  buttonDisable: boolean;
  toggleButtonDisable: Dispatch<SetStateAction<boolean>>;
}) {
  function validateInput() {
    props.subjectInputRef.current!.value == ""
      ? props.toggleButtonDisable(true)
      : props.toggleButtonDisable(false);
  }

  return (
    <>
      <FormControl>
        <FormLabel>
          Which subject would you like to copy the message to?
        </FormLabel>
        <SubjectDropDown
          subjectInputRef={props.subjectInputRef}
          validateAllInputs={validateInput}
        />
      </FormControl>
    </>
  );
}

export { MsgCopyForm };
