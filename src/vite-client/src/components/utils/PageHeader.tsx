import { Heading, Container, Text } from "@chakra-ui/react";

function PageHeader(props: { heading: string; introduction: string }) {
  return (
    <Container centerContent={true} maxW={"80%"}>
      <Heading size={"xl"} mb={5}>
        {props.heading}
      </Heading>
      <Text size={"md"}>{props.introduction}</Text>
    </Container>
  );
}

export { PageHeader };
