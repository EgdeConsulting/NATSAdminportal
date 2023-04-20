import {
  List,
  ListItem,
  Card,
  CardHeader,
  Heading,
  VStack,
  Accordion,
  AccordionItem,
  AccordionButton,
  AccordionPanel,
  AccordionIcon,
  Divider,
  Button,
  HStack,
  ButtonGroup,
} from "@chakra-ui/react";
import { useState, useEffect, memo, useContext } from "react";
import { LoadingSpinner, MsgViewContext } from "components";

function SubjectHierarchy() {
  const [subjects, setSubjects] = useState<SubjectProps[]>([]);
  const [loading, setLoading] = useState(true);

  /**
   * The "defaultIndex" property needs a newly initiated number
   * array with one entry to work properly.
   * Value 0: Expanded
   * Value 1: Collapsed
   */
  const [accIndex, setAccIndex] = useState<number[]>([1]);
  const viewContext = useContext(MsgViewContext);
  interface SubjectProps {
    name: string;
    subSubjects: SubjectProps[];
  }

  useEffect(() => {
    getSubjects();
  }, [subjects.length == 0]);

  function getSubjects() {
    fetch("/api/subjectHierarchy")
      .then((res) => res.json())
      .then((data) => {
        setSubjects(data);
        setLoading(false);
      });
  }

  // Consider adding alternating background color for each element

  const HierarchyList = memo(
    ({
      parent,
      padding,
    }: {
      parent: SubjectProps;
      padding: number;
    }): JSX.Element => {
      return (
        <>
          <List spacing={1} pl={padding}>
            <Accordion allowMultiple defaultIndex={accIndex}>
              <AccordionItem>
                {parent.subSubjects != undefined ? (
                  <>
                    <AccordionButton>
                      <ListItem>{parent.name}</ListItem>
                      <AccordionIcon />
                    </AccordionButton>
                    <AccordionPanel pb={2} pt={0}>
                      {parent.subSubjects != undefined &&
                        parent.subSubjects.map((child: SubjectProps) => {
                          return (
                            <HierarchyList
                              key={parent.name + child.name}
                              parent={child}
                              padding={padding + 5}
                            />
                          );
                        })}
                    </AccordionPanel>
                  </>
                ) : (
                  <ListItem>{parent.name}</ListItem>
                )}
              </AccordionItem>
            </Accordion>
          </List>
        </>
      );
    }
  );
  return (
    <>
      {loading ? (
        <Card
          variant={"outline"}
          w={"100%"}
          h={"92vh"}
          p={"absolute"}
          mt={"0 !important"}
        >
          <CardHeader>
            <Heading size={"md"}>Subject Hierarchy</Heading>
          </CardHeader>
          <LoadingSpinner />
        </Card>
      ) : (
        <Card
          overflowY={"auto"}
          variant={"outline"}
          w={"100%"}
          mt={"0 !important"}
          p={"absolute"}
          h={viewContext.isVisible ? "31vh" : "92vh"}
          pb={4}
        >
          <CardHeader>
            <HStack spacing={"auto"}>
              <Heading size={"md"}>Subject Hierarchy</Heading>
              <ButtonGroup>
                <Button onClick={() => setAccIndex([0])}>Expand all</Button>
                <Button onClick={() => setAccIndex([1])}>Collapse all</Button>
              </ButtonGroup>
            </HStack>

            <Divider w={"100%"} mt={2} />
          </CardHeader>

          {subjects.map((subject: SubjectProps, index: number) => {
            return (
              <VStack key={index} mx={4} alignItems={"left"}>
                <HierarchyList parent={subject} padding={0} />
              </VStack>
            );
          })}
        </Card>
      )}
    </>
  );
}

export { SubjectHierarchy };
