import {
  Card,
  CardBody,
  HStack,
  Button,
  VStack,
  Flex,
  Spacer,
  Box,
} from "@chakra-ui/react";
import { useState } from "react";
import StickyBox from "react-sticky-box";
import {
  MsgView,
  MsgTable,
  SubjectSidebar,
  MsgPublishModal,
  MsgContextProvider,
  MsgViewContextProvider,
} from "components";

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
    <MsgContextProvider>
      <MsgViewContextProvider>
        <HStack w={"100%"} align={"stretch"} pt={2}>
          <Flex w={"100%"}>
            <Card variant={"outline"} w={"75%"} mr={-2}>
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
              <Box w={"95%"} h={"100%"} ml={4}>
                <StickyBox offsetTop={10}>
                  <MsgView />
                  <SubjectSidebar />
                </StickyBox>
              </Box>
            </VStack>
          </Flex>
        </HStack>
      </MsgViewContextProvider>
    </MsgContextProvider>
  );
}

export { HomePage };
