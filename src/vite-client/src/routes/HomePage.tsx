import { Card, CardBody, Text, HStack, Input, Button } from "@chakra-ui/react";
import { useState, useRef } from "react";
import { MsgView, SubjectSidebar, MsgPublishModal, DeleteMsgModal } from "components";

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
      getAllMessages();
      setIntervalState(setInterval(getAllMessages, 10000));
      setButtonText("Stop");
    } else {
      clearInterval(intervalState);
      setIntervalState(-1);
      setButtonText(initialButtonText);
    }
  }

  return (
    <HStack align={"stretch"} paddingTop={2}>
      <Card variant={"outline"} width={"1115px"}>
        <CardBody>
          <HStack>
            <Card border={"none"}>
              <CardBody marginTop={5}>
                <Button onClick={manageAllMessagesInterval}>
                  {buttonText}
                </Button>
              </CardBody>
            </Card>
            <Card border={"none"}>
              <CardBody marginTop={5}>
                <MsgPublishModal />
              </CardBody>
            </Card>
            <Card border={"none"}>
              <CardBody marginTop={5}>
                <DeleteMsgModal />
              </CardBody>
            </Card>
          </HStack>
          <MsgView messages={allMessages} />
        </CardBody>
      </Card>
      <SubjectSidebar />
    </HStack>
  );
}

export { HomePage };
