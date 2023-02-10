import {
  Select,
  Input,
  Button,
  FormControl,
  FormLabel,
  FormErrorMessage,
  FormHelperText,
} from "@chakra-ui/react";
import { useRef } from "react";

function MessageForm() {
  const subjectInputRef = useRef<any>(null);
  const headerInputRef = useRef<any>(null);
  const payloadInputRef = useRef<any>(null);
  function testPassingMethodWithRef() {
    console.log(
      "subj: " +
        subjectInputRef.current.value +
        ", head: " +
        headerInputRef.current.value +
        ",  payload: " +
        payloadInputRef.current.value
    );
  }
  function postNewMessage() {
    //Fields cannot be empty... problem for another day.
    fetch("/PublishFullMessage", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Payload:
          payloadInputRef.current != null
            ? payloadInputRef.current.value
            : "empty",
        Headers:
          headerInputRef.current != null
            ? headerInputRef.current.value
            : "empty",
        Subject:
          subjectInputRef.current != null
            ? subjectInputRef.current.value
            : "empty",
      }),
    });
  }

  return (
    <>
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
        <FormHelperText>
          Choose the subject you want to post your message on
        </FormHelperText>

        <FormLabel marginTop={2}>Headers</FormLabel>
        <Input type={"text"} ref={headerInputRef} placeholder={"Headers..."} />

        <FormLabel marginTop={2}>Payload</FormLabel>
        <Input
          marginBottom={4}
          type={"text"}
          ref={payloadInputRef}
          placeholder={"Enter your message..."}
        />
      </FormControl>
      <Button onClick={postNewMessage} marginRight={2} colorScheme="blue">
        Publish
      </Button>
    </>
  );
}

export { MessageForm };
