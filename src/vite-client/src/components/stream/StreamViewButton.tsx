import { Button } from "@chakra-ui/react";
import { useContext } from "react";
import {
  StreamContext,
  StreamContextType,
  StreamViewContext,
} from "components";

function StreamViewButton(props: { content: any }) {
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
    >
      View Stream Data
    </Button>
  );
}

export { StreamViewButton };
