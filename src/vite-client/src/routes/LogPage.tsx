import { Button, Card, CardBody, HStack } from "@chakra-ui/react";
import { MessageView, SubjectSidebar } from "../components";
import { useState } from "react";

function LogPage() {
  const [allMessages, setAllMessages] = useState<any[]>([]);

  function getAllMessages() {
    fetch("/api/messages") // "http://localhost:3000/message1"
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
    <HStack align="stretch" margin={2} w="80%" h="100%">
      <Card variant={"outline"}>
        <CardBody>
          <Button onClick={manageAllMessagesInterval} marginBottom={6}>
            {buttonText}
          </Button>
          <MessageView messages={allMessages} />
        </CardBody>
      </Card>
      <SubjectSidebar />
    </HStack>
  );
}

export { LogPage };
