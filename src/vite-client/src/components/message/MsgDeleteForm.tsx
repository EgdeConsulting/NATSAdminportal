import {
  Checkbox,
  FormControl,
  FormLabel,
  FormHelperText,
  Text,
} from "@chakra-ui/react";
import { Dispatch, SetStateAction, useContext } from "react";
import { MsgContext } from "components";

function MsgDeleteForm(props: {
  erase: boolean;
  setErase?: Dispatch<SetStateAction<boolean>>;
}) {
  const currentMsgContext = useContext(MsgContext);

  return (
    <>
      <FormControl>
        <FormLabel>
          Delete message with sequence number:{" "}
          <Text as={"cite"} fontSize={"md"}>
            {currentMsgContext?.currentMsg &&
              currentMsgContext.currentMsg.sequenceNumber}
          </Text>
          , on stream:{" "}
          <Text as={"cite"} fontSize={"md"}>
            {currentMsgContext?.currentMsg &&
              currentMsgContext.currentMsg.stream}
          </Text>
        </FormLabel>

        <FormHelperText mt={5}>
          Optionally erasing the content of the message.
        </FormHelperText>
        <Checkbox
          defaultChecked
          onChange={() =>
            props.setErase ? (props.erase ? false : true) : null
          }
          mt={2}
        >
          Erase Message
        </Checkbox>
      </FormControl>
    </>
  );
}

export { MsgDeleteForm };
