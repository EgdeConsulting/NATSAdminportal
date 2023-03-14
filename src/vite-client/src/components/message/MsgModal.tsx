import {
  Button,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  ModalCloseButton,
  useDisclosure,
  Heading,
  Box,
  Stack,
  StackDivider,
  Text,
} from "@chakra-ui/react";
import { useState } from "react";
import { MsgDeleteModal } from "./MsgDeleteModal";

function MsgModal(props: { content: any }) {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [messageData, setMessageData] = useState<any>([]);

  function getMessageData() {
    const queryString =
      "streamName=" +
      props.content["stream"] +
      "&sequenceNumber=" +
      props.content["sequenceNumber"];
    fetch("/api/messageData?" + queryString)
      .then((res) => res.json())
      .then((data) => {
        setMessageData(data);
      });
  }

  return (
    <>
      <Button
        onClick={() => {
          getMessageData();
          onOpen();
        }}
      >
        View Data
      </Button>

      <Modal isOpen={isOpen} onClose={onClose} isCentered={true}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Message Data</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Stack divider={<StackDivider />} spacing={4}>
              <Box>
                <Heading size={"sm"} marginBottom={2}>
                  Subject
                </Heading>
                <Text fontSize={"md"}>{messageData.subject}</Text>
              </Box>
              <Box>
                <Heading size={"sm"} marginBottom={2}>
                  Headers
                </Heading>
                {messageData.headers != undefined &&
                Object.entries(messageData.headers).length != 0 ? (
                  Object.entries(messageData.headers).map(
                    ([key, value], index: number) => (
                      <Text key={index} fontSize={"md"}>
                        {key + " : " + value}
                      </Text>
                    )
                  )
                ) : (
                  <Text fontSize={"md"}>No Headers...</Text>
                )}
              </Box>
              <Box>
                <Heading size={"sm"} marginBottom={2}>
                  Payload
                </Heading>
                <Text fontSize={"md"}>{messageData.payload}</Text>
              </Box>
              <MsgDeleteModal content={props.content} />
            </Stack>
          </ModalBody>
          <ModalFooter>
            <Button variant="ghost" mb={2} mr={3} onClick={onClose}>
              Close
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { MsgModal };
