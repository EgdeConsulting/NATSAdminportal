import {
  Card,
  CardBody,
  Text,
  VStack,
  Input,
  Button,
  Box,
} from "@chakra-ui/react";
import { useState, useRef } from "react";
import { MessageView } from "../components";

function HomePage() {
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
  const subjectInputRef = useRef<any>(null);
  const publishInputRef = useRef<any>(null);

  function postNewSubject() {
    fetch("/NewSubject", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        subject:
          subjectInputRef.current != null
            ? subjectInputRef.current.value
            : "error",
      }),
    });
  }

  function postNewMessage() {
    fetch("/PublishMessage", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        payload:
          publishInputRef.current != null
            ? publishInputRef.current.value
            : "empty",
      }),
    });
  }

  return (
    <VStack align="stretch" margin={2} w="80%" h="100%">
      <Box>
        <Card variant={"outline"}>
          <CardBody>
            <Text fontSize={"lg"}>
              Use "<b>&gt;</b>" to subscribe to all subjects:
            </Text>
            <Input
              ref={subjectInputRef}
              type={"text"}
              placeholder={"Message Subject"}
              marginTop={2}
              marginRight={2}
              width={500}
            />
            <Button onClick={postNewSubject} marginTop={-1}>
              Change Message Subject
            </Button>
          </CardBody>
        </Card>
      </Box>
      <Box>
        <Card variant={"outline"}>
          <CardBody>
            <Text fontSize={"lg"}>Publish a message onto the NATS queue:</Text>
            <Input
              ref={publishInputRef}
              type={"text"}
              placeholder={"Payload"}
              marginTop={2}
              marginRight={2}
              width={500}
            />
            <Button onClick={postNewMessage} marginTop={-1}>
              Publish Message
            </Button>
          </CardBody>
        </Card>
      </Box>
      <Box>
        <Card variant={"outline"}>
          <CardBody>
            <Button onClick={manageAllMessagesInterval} marginBottom={6}>
              {buttonText}
            </Button>
            <hr />
            <MessageView messages={allMessages} />
          </CardBody>
        </Card>
      </Box>
    </VStack>
  );
}

export { HomePage };
