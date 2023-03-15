import {
  Card,
  CardBody,
  Text,
  HStack,
  Button,
  VStack,
  Flex,
  Spacer,
} from "@chakra-ui/react";
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
        <HStack w={"100%"} align={"stretch"} pt={2}>
          <Flex w={"100%"}>
            <Card variant={"outline"} w={"75%"} mr={2}>
              <CardBody>
                <HStack>
                  <Card border={"none"}>
                    <CardBody mt={5}>
                      <Button onClick={manageAllMessagesInterval}>
                        {buttonText}
                      </Button>
                    </CardBody>
                  </Card>
                  <Card border={"none"}>
                    <CardBody mt={5}>
                      <MsgPublishModal />
                    </CardBody>
                  </Card>
                </HStack>
                <MsgTable messages={allMessages} />
              </CardBody>
            </Card>
            <Spacer />
            <VStack w={"25%"} h={"100%"} mr={2}>
              <MsgView />
              <SubjectSidebar />
            </VStack>
          </Flex>
        </HStack>
      </MsgViewContextProvider>
    </MsgContextProvider>
  );
}

export { HomePage };
