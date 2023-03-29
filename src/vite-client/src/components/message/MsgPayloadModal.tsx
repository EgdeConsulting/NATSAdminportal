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
  useEditable,
} from "@chakra-ui/react";
import { IPayload, LoadingSpinner, MsgContext } from "components";
import { useContext, useState } from "react";

function MsgPayloadModal() {
  const currentMsgContext = useContext(MsgContext);
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [payload, setPayload] = useState<IPayload>({ data: "" });
  const [loading, setLoading] = useState(false);

  function getPayload() {
    setLoading(true);
    const msg = currentMsgContext.currentMsg;
    if (msg.stream && msg.sequenceNumber) {
      const queryString =
        "streamName=" + msg.stream + "&sequenceNumber=" + msg.sequenceNumber;
      fetch("/api/specificPayload?" + queryString)
        .then((res) => res.json())
        .then((data) => {
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
        View all
      </Button>

      <Modal isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Payload</ModalHeader>
          <ModalCloseButton />
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
