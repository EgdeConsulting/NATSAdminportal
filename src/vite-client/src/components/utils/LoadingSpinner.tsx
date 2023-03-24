import { Container, Center, Spinner } from "@chakra-ui/react";

function LoadingSpinner() {
  return (
    <Container centerContent={true}>
      <Center height={"300px"} ml={-6}>
        <Spinner
          size={"xl"}
          thickness={"4px"}
          speed={"0.45s"}
          emptyColor={"gray.200"}
          color={"gray.600"}
        ></Spinner>
      </Center>
    </Container>
  );
}

export { LoadingSpinner };
