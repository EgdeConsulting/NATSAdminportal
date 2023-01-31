import { useRef, useState, useEffect } from "react";
import {
  ChakraProvider,
  Box,
  theme,
  Card,
  Input,
  ColorModeProvider,
  CSSReset,
  CardBody,
  Text,
  Button,
} from "@chakra-ui/react";
import { Topbar } from "./components";

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
        <Card margin={2} variant={"outline"}>
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
        <Card margin={2} variant={"outline"}>
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
        <Card margin={2} variant={"outline"}>
          <CardBody>
            <Button onClick={manageAllMessagesInterval} marginBottom={6}>
              {buttonText}
            </Button>
            <hr />
            {allMessages.length != 0 &&
              allMessages.map((item, index) => {
                return (
                  <Box key={index}>
                    <Text fontSize={"md"} marginTop={2}>
                      #{index} Message Subject:{" "}
                      <code>{item.messageSubject}</code>
                    </Text>
                    <Text fontSize={"sm"}>
                      Timestamp: <code>{item.messageTimestamp}</code>
                    </Text>
                    <Text fontSize={"sm"}>
                      Acknowledgement: <code>{item.messageAck}</code>
                    </Text>
                    <Text fontSize={"sm"} marginBottom={2}>
                      Payload:{" "}
                      <code>
                        {typeof item.messagePayload == "string"
                          ? item.messagePayload
                          : JSON.stringify(item.messagePayload)}
                      </code>
                    </Text>
                    <hr />
                  </Box>
                );
              })}
          </CardBody>
        </Card>
      </ColorModeProvider>
    </ChakraProvider>
  );
}

export { App };
