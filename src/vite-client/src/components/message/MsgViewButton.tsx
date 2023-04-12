import { Button } from "@chakra-ui/react";
import { useContext } from "react";
import { MsgContext, MsgContextType, MsgViewContext } from "components";

function MsgViewButton(props: { content: any }) {
  const { changeCurrentMsg } = useContext(MsgContext) as MsgContextType;
  const { changeVisibility } = useContext(MsgViewContext);
  return (
    <Button
      onClick={() => {
        changeCurrentMsg({
          sequenceNumber: props.content.sequenceNumber,
          stream: props.content.stream,
          subject: props.content.subject,
        });
        changeVisibility(true);
      }}
      variant={"darkerBackground"}
    >
      View Message
    </Button>
  );
}

export { MsgViewButton };
