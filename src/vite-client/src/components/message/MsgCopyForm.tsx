import { FormControl, FormLabel } from "@chakra-ui/react";
import { SubjectDropDown } from "components/subject";
import { Dispatch, LegacyRef, SetStateAction } from "react";

function MsgCopyForm(props: {
  content: any;
  subjectInputRef: any; //LegacyRef<HTMLSelectElement>;
  buttonDisable: boolean;
  toggleButtonDisable: Dispatch<SetStateAction<boolean>>;
}) {
  function validateInput() {
    props.subjectInputRef.current.value == ""
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
