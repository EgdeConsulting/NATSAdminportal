import { Button, Card, CardBody, Box, Text, VStack } from "@chakra-ui/react";
import { MessageView } from "../components";
import { useState } from "react";

const Log = () => {
  const [allMessages, setAllMessages] = useState<any[]>([]);

  function getAllMessages() {
    fetch("/LastMessages") // "http://localhost:3000/message1"
      .then((res: any) => res.json())
      .then((data) => {
        setAllMessages(data);
      });
  }

  const initialButtonText: string = "Get all Messages";
  const [buttonText, setButtonText] = useState(initialButtonText);
  const [intervalState, setIntervalState] = useState(-1);

  function manageAllMessagesInterval() {
    if (intervalState == -1) {
      setIntervalState(setInterval(getAllMessages, 1000));
      setButtonText("Stop");
    } else {
      clearInterval(intervalState);
      setIntervalState(-1);
      setButtonText(initialButtonText);
    }
  }

  return (
    <VStack align="stretch" margin={2} w="80%" h="100%">
      <Card variant={"outline"}>
        <CardBody>
          <Button onClick={manageAllMessagesInterval} marginBottom={6}>
            {buttonText}
          </Button>
          <hr />
          <MessageView messages={allMessages} />
        </CardBody>
      </Card>
    </VStack>
  );
};

export { Log };
