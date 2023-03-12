import { Card, CardBody, Text, HStack, Button, VStack } from "@chakra-ui/react";
import { useContext, useState } from "react";
import {
  MsgView,
  MsgTable,
  SubjectSidebar,
  MsgPublishModal,
  MsgContextProvider,
  MsgContext,
  MsgViewContextProvider,
} from "components";

function HomePage() {
  const [allMessages, setAllMessages] = useState<any[]>([]);
  const currentMsgContex = useContext(MsgContext);

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

  const [showMsg, setShowMsg] = useState(false);

  return (
    <MsgContextProvider>
      <MsgViewContextProvider>
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
              </HStack>
              <MsgTable messages={allMessages} />
            </CardBody>
          </Card>
          <VStack>
            <MsgView />
            <SubjectSidebar />
          </VStack>
        </HStack>
      </MsgViewContextProvider>
    </MsgContextProvider>
  );
}

export { HomePage };
