import { useRef } from "react";
import {
  Card,
  CardBody,
  Button,
  Text,
  Input,
  VStack,
  Box,
} from "@chakra-ui/react";

const Settings = () => {
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
        Subject:
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
        Payload:
          publishInputRef.current != null
            ? publishInputRef.current.value
            : "empty",
      }),
    });
  }

  return (
    <VStack align="stretch" margin={2} w="80%" h="100%">
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
    </VStack>
  );
};

export { Settings };