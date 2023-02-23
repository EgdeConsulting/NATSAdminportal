import { Card, CardBody, Text, HStack, Input, Button } from "@chakra-ui/react";
import { useState, useRef } from "react";
import { MessageView, SubjectSidebar, MessageModal } from "components";

function HomePage() {
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
  const subjectInputRef = useRef<any>(null);
  const publishInputRef = useRef<any>(null);

  function postNewSubject() {
    fetch("/api/newSubject", {
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

  return (
    <HStack align={"stretch"} paddingTop={2}>
      <Card variant={"outline"} width={"1115px"}>
        <CardBody>
          <HStack>
            <Card border={"none"} marginLeft={-3}>
              <CardBody marginTop={-5}>
                <Text fontSize={"lg"}>
                  Use "<b>&gt;</b>" to subscribe to all subjects:
                </Text>
                <Input
                  ref={subjectInputRef}
                  type={"text"}
                  placeholder={"Message Subject"}
                  marginTop={2}
                  marginRight={2}
                  width={280}
                />
                <Button onClick={postNewSubject} marginTop={-1}>
                  Change Message Subject
                </Button>
              </CardBody>
            </Card>
            <Card border={"none"}>
              <CardBody marginTop={5}>
                <Button onClick={manageAllMessagesInterval}>
                  {buttonText}
                </Button>
              </CardBody>
            </Card>
            <Card border={"none"}>
              <CardBody marginTop={5}>
                <MessageModal />
              </CardBody>
            </Card>
          </HStack>
          <MessageView messages={allMessages} />
        </CardBody>
      </Card>
      <SubjectSidebar />
    </HStack>
  );
}

export { HomePage };
