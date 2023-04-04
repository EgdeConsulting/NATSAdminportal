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
} from "@chakra-ui/react";
import { useState, useEffect, memo } from "react";
import { LoadingSpinner } from "components";

function SubjectHierarchy() {
  const [subjects, setSubjects] = useState<[]>([]);
  //Wakanda datatype is this?? [] with {name, subsubjects?} ??
  // interface within here probably
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getSubjects();
  }, [subjects.length == 0]);

  function getSubjects() {
    fetch("/api/subjectHierarchy")
      .then((res) => res.json())
      .then((data) => {
        setSubjects(data);
        setLoading(false);
        console.log(data);
      });
  }
  // WHATSUP WITH ALL THESE STACK DIVIDERS??????????????????
  const HierarchyList = memo(
    ({ parent, padding }: { parent: any; padding: number }): JSX.Element => {
      return (
        <>
          <List spacing={3} m={1} pl={padding}>
            <Accordion allowMultiple>
              <AccordionItem>
                {parent.subSubjects != undefined ? (
                  <AccordionButton>
                    <ListItem>{parent.name}</ListItem>
                    <AccordionIcon />
                  </AccordionButton>
                ) : (
                  <ListItem>{parent.name}</ListItem>
                )}
                <AccordionPanel pb={4}>
                  {parent.subSubjects != undefined &&
                    parent.subSubjects.map((child: any) => {
                      console.log(child.subSubjects);
                      return (
                        <HierarchyList
                          key={parent.name + child.name}
                          parent={child}
                          padding={padding + 5}
                        />
                      );
                    })}
                </AccordionPanel>
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
        <Card variant={"outline"} w={"100%"} mt={"0 !important"}>
          <CardHeader>
            <Heading size={"md"}>Subject Hierarchy</Heading>
          </CardHeader>
          <LoadingSpinner />
        </Card>
      ) : (
        <Card variant={"outline"} w={"100%"} mt={"0 !important"}>
          <CardHeader>
            <Heading size={"md"}>Subject Hierarchy</Heading>
          </CardHeader>

          {subjects.map((subject: any, index: number) => {
            return (
              <VStack key={index} ml={4} mb={3} alignItems={"left"}>
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
