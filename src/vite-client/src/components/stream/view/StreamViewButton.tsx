import { Button } from "@chakra-ui/react";
import { useContext } from "react";
import {
  StreamContext,
  StreamContextType,
  StreamViewContext,
} from "components";

function StreamViewButton(props: { content: Record<string, any> }) {
  const { changeCurrentStream: changeCurrentStream } = useContext(
    StreamContext
  ) as StreamContextType;
  const { changeVisibility } = useContext(StreamViewContext);
  return (
    <Button
      onClick={() => {
        changeCurrentStream({
          name: props.content.name,
        });
        changeVisibility(true);
      }}
      variant={"darkerBackground"}
    >
      View Stream
    </Button>
  );
}

export { StreamViewButton };
