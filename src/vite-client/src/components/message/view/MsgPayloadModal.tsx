import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  useDisclosure,
  Button,
  Text,
  Divider,
  Center,
} from "@chakra-ui/react";
import { PayloadProps, LoadingSpinner, MsgContext } from "components";
import { useContext, useState } from "react";

function MsgPayloadModal() {
  const currentMsgContext = useContext(MsgContext);
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [payload, setPayload] = useState<PayloadProps>({ data: "" });
  const [loading, setLoading] = useState(false);

  function getPayload() {
    setLoading(true);
    const msg = currentMsgContext.currentMsg;
    if (msg.stream && msg.sequenceNumber) {
      const queryString =
        "streamName=" + msg.stream + "&sequenceNumber=" + msg.sequenceNumber;
      fetch("/api/specificPayload?" + queryString)
        .then((res) => res.json())
        .then((data: PayloadProps) => {
          setPayload(data);
          setLoading(false);
        });
    }
  }

  return (
    <>
      <Button
        onClick={() => {
          onOpen();
          getPayload();
        }}
      >
        View full Payload
      </Button>

      <Modal size={"2xl"} isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Payload</ModalHeader>
          <ModalCloseButton />
          <Center>
            <Divider w={"93%"} />
          </Center>

          {loading ? (
            <ModalBody>
              <LoadingSpinner />
            </ModalBody>
          ) : (
            <ModalBody>
              <Text>{payload.data}</Text>
            </ModalBody>
          )}

          <ModalFooter>
            <Button colorScheme="blue" mr={3} onClick={onClose}>
              Close
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { MsgPayloadModal };
