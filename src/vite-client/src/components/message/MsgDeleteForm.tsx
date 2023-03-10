import {
  Checkbox,
  FormControl,
  FormLabel,
  FormHelperText,
  Text,
} from "@chakra-ui/react";
import { Dispatch, SetStateAction } from "react";

function MsgDeleteForm(props: {
  content: any;
  erase: boolean;
  setErase?: Dispatch<SetStateAction<boolean>>;
}) {
  return (
    <>
      <FormControl>
        <FormLabel>
          Delete message with sequence number:{" "}
          <Text as={"cite"} fontSize={"md"}>
            {props.content["sequenceNumber"]}
          </Text>
          , on stream:{" "}
          <Text as={"cite"} fontSize={"md"}>
            {props.content["stream"]}
          </Text>
        </FormLabel>

        <FormHelperText marginTop={5}>
          Optionally erasing the content of the message.
        </FormHelperText>
        <Checkbox
          defaultChecked
          onChange={() =>
            props.setErase ? (props.erase ? false : true) : null
          }
          marginTop={2}
        >
          Erase Message
        </Checkbox>
      </FormControl>
    </>
  );
}

export { MsgDeleteForm };
