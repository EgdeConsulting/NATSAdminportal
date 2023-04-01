import {
  Button,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalCloseButton,
  useDisclosure,
  ModalFooter,
  Tooltip,
} from "@chakra-ui/react";
import { ChatIcon } from "@chakra-ui/icons";
import { MsgPublishForm, ActionConfirmation, IHeaderProps } from "components";
import { useRef, useState } from "react";

function MsgPublishModal() {
  const [buttonDisable, toggleButtonDisable] = useState<boolean>(true);
  const { isOpen, onOpen, onClose } = useDisclosure();
  const {
    isOpen: isOpenAC,
    onOpen: onOpenAC,
    onClose: onCloseAC,
  } = useDisclosure();

  const [headerList, setHeaderList] = useState<IHeaderProps[]>([
    { name: "", value: "" },
  ]);
  const subjectInputRef = useRef<HTMLSelectElement>(null);
  const payloadInputRef = useRef<HTMLInputElement>(null);

  function postNewMessage() {
    let bodyContent;
    if (
      !process.env.NODE_ENV ||
      process.env.NODE_ENV === "development+json-server"
    ) {
      const id = Math.round(Math.random() * (10000 - 100));
      bodyContent = {
        id: id,
        sequenceNumber: id,
        timestamp: new Date(),
        stream: "stream1",
        subject: subjectInputRef.current!.value,
        headers: headerList,
        payload: payloadInputRef.current!.value,
      };
    } else {
      bodyContent = {
        subject: subjectInputRef.current!.value,
        headers: headerList,
        payload: { data: payloadInputRef.current!.value },
      };
    }
    fetch("/api/newMessage", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify(bodyContent),
    }).then((res) => {
      if (res.ok) {
        subjectInputRef.current!.value = "";
        setHeaderList([{ name: "", value: "" }]);
        payloadInputRef.current!.value = "";
      } else {
        alert(
          "An error occurred while publishing the message: " + res.statusText
        );
      }
    });
  }

  return (
    <>
      <Button
        margin={2}
        size={"md"}
        onClick={() => {
          setHeaderList([{ name: "", value: "" }]);
          onOpen();
        }}
        aria-label="Publish a message"
        leftIcon={<ChatIcon />}
      >
        Publish new Message
      </Button>

      <Modal size={"md"} isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Publish message</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <MsgPublishForm
              buttonDisable={buttonDisable}
              toggleButtonDisable={toggleButtonDisable}
              subjectInputRef={subjectInputRef}
              payloadInputRef={payloadInputRef}
              headerList={headerList}
              setHeaderList={setHeaderList}
            />
          </ModalBody>
          <ModalFooter>
            <Tooltip
              isDisabled={!buttonDisable}
              hasArrow
              label="Select subject, provide at least 1 header and provide payload. ASCII characters only"
              aria-label="Reqs for publish"
            >
              <Button
                mb={2}
                isDisabled={buttonDisable}
                colorScheme="blue"
                onClick={onOpenAC}
              >
                Publish
              </Button>
            </Tooltip>
            <Button
              mb={2}
              ml={2}
              variant="ghost"
              onClick={() => {
                setHeaderList([{ name: "", value: "" }]);
                onClose();
              }}
            >
              Close
            </Button>
            <ActionConfirmation
              action={postNewMessage}
              onClose={() => {
                onCloseAC();
              }}
              isOpen={isOpenAC}
            />
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

export { MsgPublishModal };
