import { Heading, Container, Text } from "@chakra-ui/react";

function PageHeader(props: {
  centerContent: boolean;
  heading: string;
  introduction: string;
}) {
  return (
    <Container m={0} centerContent={props.centerContent} maxW={"100%"}>
      <Heading as={"h1"} size={"xl"} mb={5}>
        {props.heading}
      </Heading>
      <Text size={"md"}>{props.introduction}</Text>
    </Container>
  );
}

export { PageHeader };
