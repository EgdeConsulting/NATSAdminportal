import {
  Select,
  Input,
  FormControl,
  FormLabel,
  FormErrorMessage,
  FormHelperText,
} from "@chakra-ui/react";
import { useRef } from "react";

function ModalForm() {
  const subjectInputRef = useRef<any>(null);
  const headerInputRef = useRef<any>(null);
  const payloadInputRef = useRef<any>(null);

  return (
    <FormControl isRequired>
      <FormLabel>Subject</FormLabel>
      {/* Whenever we get the subjects in here this can be its own component? */}
      <Select ref={subjectInputRef} placeholder="Select a subject">
        <option>subject.A.1</option>
        <option>subject.A.2</option>
        <option>subject.B.1</option>
        <option>subject.B.2</option>
        <option>subject.C.1</option>
        <option>subject.C.2</option>
        <option>subject2.A.1</option>
        <option>subject2.A.2</option>
      </Select>
      <FormLabel marginTop={2}>Headers</FormLabel>
      <Input type={"text"} ref={headerInputRef} placeholder={"Headers..."} />
      <FormLabel marginTop={2}>Payload</FormLabel>
      <Input
        type={"text"}
        ref={payloadInputRef}
        placeholder={"Enter your message..."}
      />
    </FormControl>
  );
}

export { ModalForm };
