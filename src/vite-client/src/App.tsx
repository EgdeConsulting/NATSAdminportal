import { useRef, useState, useEffect } from "react";
import {
  ChakraProvider,
  Box,
  theme,
  Flex,
  Card,
  Input,
  ColorModeProvider,
  CSSReset,
  CardBody,
  Text,
  Button,
  HStack,
  VStack,
  StackDivider,
} from "@chakra-ui/react";
import { Topbar, MessageView, SideBar } from "./components";

function App() {
  const subjectInputRef = useRef<any>(null);
  const publishInputRef = useRef<any>(null);

  // const [subject, setSubject] = useState("empty");
  // const [payload, setPayload] = useState("empty");
  // const [count, setCount] = useState(0);

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

  // async function loadData() {
  //   try {
  //     fetch("https://localhost:7116/LastMessage")
  //       .then((res) => res.json())
  //       .then((data) => {
  //         setSubject(data.messageSubject);
  //         setPayload(data.messagePayload);
  //       });

  //     fetch("https://localhost:7116/MessageCount")
  //       .then((res) => res.text())
  //       .then((text) => setCount(+text));
  //   } catch (e) {
  //     console.log(e);
  //   }
  // }

  // useEffect(() => {
  //   loadData();
  //   setInterval(loadData, 10000);
  // });

  const [allMessages, setAllMessages] = useState<any[]>([]);

  function getAllMessages() {
    fetch("/LastMessages") // "http://localhost:3000/message1"
      .then((res) => res.json())
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
    <ChakraProvider theme={theme}>
      <ColorModeProvider>
        <CSSReset />
        <Topbar />
        <HStack align="stretch">
          <SideBar />
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
                <Text fontSize={"lg"}>
                  Publish a message onto the NATS queue:
                </Text>
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
        </HStack>
      </ColorModeProvider>
    </ChakraProvider>
  );
}

export { App };
